import axios from "axios";

const api = axios.create({
  baseURL: "/api", // proxy will forward to http://localhost:5196
  headers: { "Content-Type": "application/json" }
});

export const Players = {
  getAll: () => api.get("/players").then(r => r.data),
  getById: (id) => api.get(`/players/${id}`).then(r => r.data),
  search: (term) => api.get(`/players/search/${encodeURIComponent(term)}`).then(r => r.data),
  create: (p) => api.post("/players", p).then(r => r.data),
  update: (id, p) => api.put(`/players/${id}`, p).then(r => r.status === 204),
  delete: (id) => api.delete(`/players/${id}`).then(r => r.status === 204)
};

export const Teams = {
  getAll: () => api.get("/team").then(r => r.data),
  getById: (id) => api.get(`/team/${id}`).then(r => r.data),
  create: (t) => api.post("/team", t).then(r => r.data),
  // team players / roster endpoint
  roster: (teamId) => api.get(`/teamplayers/roster/${teamId}`).then(r => r.data)
};

export const Platforms = {
  getAll: () => api.get("/platform").then(r => r.data),
  getById: (id) => api.get(`/platform/${id}`).then(r => r.data)
};

export const Users = {
  getAll: () => api.get("/user").then(r => r.data),
  getById: (id) => api.get(`/user/${id}`).then(r => r.data)
};