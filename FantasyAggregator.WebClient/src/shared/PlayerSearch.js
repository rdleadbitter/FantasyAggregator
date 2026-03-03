import React, { useState } from "react";
import { Players } from "../api";
import { Box, TextField, List, ListItem, ListItemText, Button } from "@mui/material";

export default function PlayerSearch({ onSelect }) {
  const [q, setQ] = useState("");
  const [results, setResults] = useState([]);

  const doSearch = () => {
    if (!q) return;
    Players.search(q).then(setResults).catch(() => setResults([]));
  };

  return (
    <Box>
      <TextField size="small" placeholder="Search players (e.g. Mahomes)" value={q} onChange={(e) => setQ(e.target.value)} />
      <Button sx={{ ml: 1 }} variant="contained" onClick={doSearch}>Search</Button>

      <List>
        {results.map(p => (
          <ListItem key={p.playerId ?? p.PlayerId} secondaryAction={
            <Button onClick={() => onSelect(p)} variant="outlined" size="small">Add</Button>
          }>
            <ListItemText primary={`${p.fullName ?? p.FullName}`} secondary={`${p.position ?? p.Position} — ${p.teamAbbrev ?? p.TeamAbbrev}`} />
          </ListItem>
        ))}
      </List>
    </Box>
  );
}