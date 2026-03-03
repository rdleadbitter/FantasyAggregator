// src/pages/TeamView.js
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Teams } from "../api";
import { Typography, Box, Button, List, ListItem, ListItemText, Stack, IconButton } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import PlayerSearch from "../shared/PlayerSearch";

function normalizeRosterItem(item) {
  if (Array.isArray(item) && item.length === 2) {
    return { tp: item[0], name: item[1] };
  }

  const getProp = (obj, ...keys) => {
    for (const k of keys) {
      if (obj == null) continue;
      if (k in obj) return obj[k];
      const kLower = Object.keys(obj).find(x => x.toLowerCase() === k.toLowerCase());
      if (kLower) return obj[kLower];
    }
    return undefined;
  };

  const tp = {
    teamPlayerId: getProp(item, "teamPlayerId", "TeamPlayerId"),
    teamId: getProp(item, "teamId", "TeamId"),
    playerId: getProp(item, "playerId", "PlayerId"),
    rosterSlot: getProp(item, "rosterSlot", "RosterSlot"),
    acquiredOn: getProp(item, "acquiredOn", "AcquiredOn")
  };

  const name = getProp(item, "playerName", "PlayerName") ??
               getProp(item, "name", "Name") ?? "";

  return { tp, name };
}

const POSITION_COLORS = {
  QB: "#1f77b4",
  RB: "#ff7f0e",
  WR: "#2ca02c",
  TE: "#d62728",
  K:  "#9467bd",
  DEF:"#8c564b",
  FLEX:"#e377c2",
  IR: "#7f7f7f",
};

function getPositionAndColor(slot) {
  if (slot == null) return { posText: "(no slot)", color: "#999999" };
  const raw = String(slot).trim();
  const posToken = raw.split(/[\/\s,-]+/)[0].toUpperCase();
  const color = POSITION_COLORS[posToken] ?? "#999999";
  return { posText: raw, color, posToken };
}

export default function TeamView() {
  const { id } = useParams();
  const teamId = parseInt(id, 10);
  const [roster, setRoster] = useState([]);
  const [team, setTeam] = useState(null);
  const [loading, setLoading] = useState(true);
  const [droppingIds, setDroppingIds] = useState(new Set());

  const loadRoster = async () => {
    try {
      setLoading(true);
      const data = await Teams.roster(teamId);
      const normalized = (data || []).map(normalizeRosterItem);
      setRoster(normalized);
    } catch (e) {
      console.error("Failed to load roster", e);
      setRoster([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    Teams.getById(teamId).then(setTeam).catch(() => setTeam(null));
    loadRoster();
  }, [teamId]);

  const exportCsv = () => {
    const csvLines = [
      "TeamPlayerId,PlayerId,PlayerName,RosterSlot,AcquiredOn"
    ];
    for (const { tp, name } of roster) {
      const tpid = tp.teamPlayerId ?? tp.TeamPlayerId ?? "";
      const pid = tp.playerId ?? tp.PlayerId ?? "";
      const slot = tp.rosterSlot ?? tp.RosterSlot ?? "";
      const acq = tp.acquiredOn ?? tp.AcquiredOn ?? "";
      const safeName = (name ?? "").replace(/"/g, '""');
      csvLines.push(`${tpid},${pid},"${safeName}",${slot},${acq}`);
    }
    const csv = csvLines.join("\n");
    const blob = new Blob([csv], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `roster_${teamId}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  };

  // Drop (delete) a player from the team
  const handleDrop = async (teamPlayerId, fallbackTp) => {
    if (!teamPlayerId && !fallbackTp) {
      alert("Cannot drop: missing identifier.");
      return;
    }

    const confirmed = window.confirm("Drop this player from the team? This action cannot be undone.");
    if (!confirmed) return;

    // optimistic UI: mark as dropping
    setDroppingIds(prev => new Set([...prev, teamPlayerId || fallbackTp.playerId]));

    try {
      if (teamPlayerId) {
        const resp = await fetch(`/api/teamplayers/${teamPlayerId}`, { method: "DELETE" });
        if (!resp.ok) {
          const txt = await resp.text();
          throw new Error(txt || resp.statusText);
        }
      } else {
        // fallback: some APIs accept DELETE with JSON body or POST to a "delete" action
        const resp = await fetch(`/api/teamplayers`, {
          method: "DELETE",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(fallbackTp)
        });
        if (!resp.ok) {
          const txt = await resp.text();
          throw new Error(txt || resp.statusText);
        }
      }

      // reload roster after successful drop
      await loadRoster();
    } catch (e) {
      console.error("Drop failed:", e);
      alert("Failed to drop player: " + (e.message || e));
    } finally {
      setDroppingIds(prev => {
        const copy = new Set(prev);
        copy.delete(teamPlayerId || fallbackTp.playerId);
        return copy;
      });
    }
  };

  const onSelectPlayer = (player) => {
    console.log("Selected player object:", player);
    const newTp = {
      teamId: teamId,
      playerId: player.playerId ?? player.PlayerId,
      rosterSlot: player.position ?? player.Position ?? player.pos ?? "Bench",
    };
    fetch("/api/teamplayers", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newTp)
    })
      .then(async r => {
        if (!r.ok) {
          const text = await r.text();
          throw new Error(text || r.statusText);
        }
        return r.json();
      })
      .then(() => loadRoster())
      .catch(e => {
        console.error("Add failed:", e);
        alert("Add failed: " + (e.message || e));
      });
  };

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">
          Team Roster: {team?.teamName ?? team?.TeamName ?? `Team ${teamId}`}
        </Typography>
        <Button variant="contained" onClick={exportCsv}>Export CSV</Button>
      </Stack>

      {loading ? <Typography>Loading roster…</Typography> : null}

      <List>
        {roster.length === 0 && !loading && <Typography>No players in roster.</Typography>}
        {roster.map(({ tp, name }) => {
          const slot = tp.rosterSlot ?? tp.RosterSlot ?? "(no slot)";
          const added = tp.acquiredOn ?? tp.AcquiredOn ?? "";
          const displayName = name || `Player ${tp.playerId ?? tp.PlayerId}`;
          const key = tp.teamPlayerId ?? tp.TeamPlayerId ?? `${tp.playerId ?? tp.PlayerId}-${teamId}`;

          const { posText, color } = getPositionAndColor(slot);
          const teamPlayerId = tp.teamPlayerId ?? tp.TeamPlayerId;
          const isDropping = droppingIds.has(teamPlayerId || tp.playerId);

          return (
            <ListItem key={key} sx={{ alignItems: "flex-start", display: "flex", justifyContent: "space-between" }}>
              <Box sx={{ display: "flex", alignItems: "center" }}>
                <Box sx={{ display: "flex", alignItems: "center", mr: 2, minWidth: 72 }}>
                  <Box
                    title={posText}
                    sx={{
                      width: 16,
                      height: 16,
                      borderRadius: 2,
                      backgroundColor: color,
                      boxShadow: "0 0 0 1px rgba(0,0,0,0.06) inset",
                      mr: 1
                    }}
                  />
                  <Typography variant="body2" sx={{ fontWeight: 700 }}>
                    {posText}
                  </Typography>
                </Box>

                <ListItemText
                  primary={displayName}
                  secondary={`Added: ${added}`}
                />
              </Box>

              <Box sx={{ display: "flex", alignItems: "center" }}>
                <IconButton
                  aria-label="drop player"
                  onClick={() => handleDrop(teamPlayerId, { teamId, playerId: tp.playerId ?? tp.PlayerId })}
                  disabled={isDropping}
                  size="small"
                  title="Drop player"
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </Box>
            </ListItem>
          );
        })}
      </List>

      <Box mt={3}>
        <Typography variant="subtitle1">Add player to roster</Typography>
        <PlayerSearch onSelect={onSelectPlayer} />
      </Box>
    </Box>
  );
}