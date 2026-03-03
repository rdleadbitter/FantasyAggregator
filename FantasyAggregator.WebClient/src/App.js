import React from "react";
import { Routes, Route, Link } from "react-router-dom";
import { Container, AppBar, Toolbar, Typography, Button } from "@mui/material";
import Dashboard from "./pages/Dashboard";
import TeamView from "./pages/TeamView";

export default function App() {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>FantasyAggregator</Typography>
          <Button color="inherit" component={Link} to="/">Dashboard</Button>
        </Toolbar>
      </AppBar>

      <Container sx={{ mt: 3 }}>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/team/:id" element={<TeamView />} />
        </Routes>
      </Container>
    </>
  );
}