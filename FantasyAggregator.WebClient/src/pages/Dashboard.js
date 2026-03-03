// src/pages/Dashboard.js
import React, { useEffect, useState } from "react";
import { Teams, Platforms, Users } from "../api";
import {
  Grid, Card, CardContent, Typography, CardActions, Button,
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, Select, MenuItem, FormControl, InputLabel, Box
} from "@mui/material";
import { useNavigate } from "react-router-dom";

/* small tolerant helpers */
const pick = (obj, ...keys) => {
  if (!obj) return undefined;
  for (const k of keys) {
    if (k in obj && obj[k] != null) return obj[k];
  }
  return undefined;
};
const idOf = (obj) => pick(obj, "TeamId", "teamId", "Id", "id", "PlatformId", "platformId", "UserId", "userId");
const nameOf = (obj) => pick(obj,
  "Name","name","platformName","PlatformName","Username","username","DisplayName","displayName","FullName","fullName","TeamName","teamName");

/* tolerant resolver for list item id -> name mapping */
const makeMapById = (arr, idKeys = ["Id","id","PlatformId","platformId","UserId","userId"], nameKeys = ["Name","name","Username","username","DisplayName","displayName"]) => {
  const map = new Map();
  if (!Array.isArray(arr)) return map;
  for (const item of arr) {
    let id = undefined;
    for (const k of idKeys) { if (k in item && item[k] != null) { id = item[k]; break; } }
    if (id == null) {
      // try common snake or nested
      if ("platform_id" in item) id = item.platform_id;
      if ("user_id" in item) id = item.user_id;
    }
    let name = undefined;
    for (const k of nameKeys) { if (k in item && item[k] != null) { name = item[k]; break; } }
    if (name == null) {
      // try alternatives
      name = item?.title ?? item?.label ?? item?.username ?? item?.fullName ?? item?.TeamName ?? undefined;
    }
    if (id != null) map.set(String(id), String(name ?? id));
  }
  return map;
};

export default function Dashboard() {
  const [teams, setTeams] = useState([]);
  const [platforms, setPlatforms] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  const [createOpen, setCreateOpen] = useState(false);
  const [newTeam, setNewTeam] = useState({
    teamName: "",
    leagueName: "",
    platformId: "",
    userId: ""
  });

  const navigate = useNavigate();

  useEffect(() => {
    let mounted = true;
    const load = async () => {
      setLoading(true);
      try {
        const [tResp, pResp, uResp] = await Promise.all([
          Teams.getAll(),
          Platforms.getAll(),
          Users.getAll()
        ]);

        // log raw responses once for troubleshooting
        console.debug("API response - teams:", tResp);
        console.debug("API response - platforms:", pResp);
        console.debug("API response - users:", uResp);

        if (!mounted) return;
        setTeams(Array.isArray(tResp) ? tResp : (tResp?.items ?? []));
        setPlatforms(Array.isArray(pResp) ? pResp : (pResp?.items ?? []));
        setUsers(Array.isArray(uResp) ? uResp : (uResp?.items ?? []));

        // sane defaults
        setNewTeam(nt => ({
          ...nt,
          platformId: (Array.isArray(pResp) && pResp[0]) ? (pick(pResp[0], "PlatformId","platformId","id","Id") ?? "") : nt.platformId,
          userId: (Array.isArray(uResp) && uResp[0]) ? (pick(uResp[0], "UserId","userId","id","Id") ?? "") : nt.userId
        }));
      } catch (err) {
        console.error("Dashboard load error:", err);
      } finally {
        if (mounted) setLoading(false);
      }
    };
    load();
    return () => { mounted = false; };
  }, []);

  // create lookup maps (string keys)
  const platformMap = makeMapById(platforms, ["PlatformId","platformId","Id","id"], ["Name","name","platformName","PlatformName"]);
  const userMap = makeMapById(users, ["UserId","userId","Id","id"], ["Username","username","displayName","DisplayName","Name","name"]);

  const getPlatformName = (teamOrId) => {
    if (!teamOrId) return "-";
    // If team object passed, try nested platform or PlatformId
    if (typeof teamOrId === "object") {
      const nested = pick(teamOrId, "Platform", "platform");
      if (nested) {
        const name = nameOf(nested);
        if (name) return String(name);
      }
      const pid = pick(teamOrId, "PlatformId", "platformId", "platform_id");
      if (pid != null) return platformMap.get(String(pid)) ?? `Platform ${pid}`;
    }
    return platformMap.get(String(teamOrId)) ?? `Platform ${teamOrId}`;
  };

  const getUserName = (teamOrId) => {
    if (!teamOrId) return "-";
    if (typeof teamOrId === "object") {
      const nested = pick(teamOrId, "User","user");
      if (nested) {
        const name = nameOf(nested);
        if (name) return String(name);
      }
      const uid = pick(teamOrId, "UserId","userId","user_id");
      if (uid != null) return userMap.get(String(uid)) ?? `User ${uid}`;
    }
    return userMap.get(String(teamOrId)) ?? `User ${teamOrId}`;
  };

  const openCreate = () => {
    setNewTeam({
      teamName: "",
      leagueName: "",
      platformId: platforms[0]?.PlatformId ?? platforms[0]?.platformId ?? platforms[0]?.id ?? "",
      userId: users[0]?.UserId ?? users[0]?.userId ?? users[0]?.id ?? ""
    });
    setCreateOpen(true);
  };
  const closeCreate = () => setCreateOpen(false);

  const submitCreate = async () => {
    if (!newTeam.teamName || !newTeam.platformId || !newTeam.userId) {
      alert("Please enter team name, platform and owner.");
      return;
    }
    try {
      const payload = {
        TeamName: newTeam.teamName,
        LeagueName: newTeam.leagueName || null,
        PlatformId: Number(newTeam.platformId),
        UserId: Number(newTeam.userId)
      };
      if (typeof Teams?.create === "function") {
        await Teams.create(payload);
      } else {
        const resp = await fetch("/api/team", { method: "POST", headers: { "Content-Type":"application/json" }, body: JSON.stringify(payload) });
        if (!resp.ok) throw new Error("create failed " + resp.status);
      }
      const fresh = await Teams.getAll();
      setTeams(Array.isArray(fresh) ? fresh : (fresh?.items ?? []));
      setCreateOpen(false);
    } catch (err) {
      console.error("Create failed:", err);
      alert("Failed to create team. See console.");
    }
  };

  return (
    <>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Teams</Typography>
        <Button variant="contained" onClick={openCreate}>Create new team</Button>
      </Box>

      {loading && <Typography>Loading…</Typography>}

      <Grid container spacing={2}>
        {teams.map(team => {
          // tolerate team.TeamId/team.teamId
          const id = pick(team, "TeamId", "teamId", "Id", "id") ?? "unknown";
          const name = pick(team, "TeamName", "teamName") ?? String(id);
          const league = pick(team, "LeagueName", "leagueName") ?? "-";
          const platformIdOrObj = pick(team, "PlatformId", "platformId") ?? pick(team, "Platform", "platform") ?? team.PlatformId ?? team.platformId;
          const userIdOrObj = pick(team, "UserId", "userId") ?? pick(team, "User", "user") ?? team.UserId ?? team.userId;

          return (
            <Grid item key={String(id)} xs={12} sm={6} md={4}>
              <Card>
                <CardContent>
                  <Typography variant="h6">{name}</Typography>
                  <Typography color="text.secondary">League: {league}</Typography>
                  <Typography color="text.secondary">Platform: {getPlatformName(platformIdOrObj)}</Typography>
                  <Typography color="text.secondary">Owner: {getUserName(userIdOrObj)}</Typography>
                </CardContent>
                <CardActions>
                  <Button size="small" onClick={() => navigate(`/team/${id}`)}>Open Roster</Button>
                </CardActions>
              </Card>
            </Grid>
          );
        })}
        {!loading && teams.length === 0 && <Typography>No teams found.</Typography>}
      </Grid>

      <Dialog open={createOpen} onClose={closeCreate}>
        <DialogTitle>Create new team</DialogTitle>
        <DialogContent>
          <Box sx={{ display:"flex", flexDirection:"column", gap:2, mt:1, minWidth:320 }}>
            <TextField label="Team name" value={newTeam.teamName} onChange={e => setNewTeam({...newTeam, teamName: e.target.value})} />
            <TextField label="League name" value={newTeam.leagueName} onChange={e => setNewTeam({...newTeam, leagueName: e.target.value})} />
            <FormControl>
              <InputLabel>Platform</InputLabel>
              <Select value={newTeam.platformId} label="Platform" onChange={e => setNewTeam({...newTeam, platformId: e.target.value})}>
                {platforms.map(p => {
                  const pid = pick(p, "PlatformId","platformId","Id","id");
                  const label = nameOf(p) ?? String(pid);
                  return <MenuItem key={String(pid)} value={pid}>{label}</MenuItem>;
                })}
              </Select>
            </FormControl>
            <FormControl>
              <InputLabel>Owner</InputLabel>
              <Select value={newTeam.userId} label="Owner" onChange={e => setNewTeam({...newTeam, userId: e.target.value})}>
                {users.map(u => {
                  const uid = pick(u, "UserId","userId","Id","id");
                  const label = nameOf(u) ?? String(uid);
                  return <MenuItem key={String(uid)} value={uid}>{label}</MenuItem>;
                })}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={closeCreate}>Cancel</Button>
          <Button variant="contained" onClick={submitCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}