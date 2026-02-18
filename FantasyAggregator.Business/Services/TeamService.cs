using System.Collections.Generic;
using FantasyAggregatorApp.Models;
using FantasyAggregatorApp.Repositories;

namespace FantasyAggregator.Business.Services
{
    public class TeamService
    {
        private readonly TeamRepository _repo = new TeamRepository();

        public IEnumerable<Team> GetAll() => _repo.GetAll();
        public Team Get(int id) => _repo.GetById(id);
        public int Create(Team t) => _repo.Create(t);
        public bool Update(Team t) => _repo.Update(t);
        public bool Delete(int id) => _repo.Delete(id);
    }
}
