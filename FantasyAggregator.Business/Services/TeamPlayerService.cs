using System;
using System.Collections.Generic;
using FantasyAggregatorApp.Models;
using FantasyAggregatorApp.Repositories;

namespace FantasyAggregator.Business.Services
{
    public class TeamPlayerService
    {
        private readonly TeamPlayerRepository _repo = new TeamPlayerRepository();

        public IEnumerable<TeamPlayer> GetAll() => _repo.GetAll();
        public TeamPlayer Get(int id) => _repo.GetById(id);
        public int Create(TeamPlayer tp) => _repo.Create(tp);
        public bool Update(TeamPlayer tp) => _repo.Update(tp);
        public bool Delete(int id) => _repo.Delete(id);

        // rosters
        public IEnumerable<(TeamPlayer, string)> GetRosterWithNames(int teamId) => _repo.GetRosterWithNames(teamId);
    }
}
