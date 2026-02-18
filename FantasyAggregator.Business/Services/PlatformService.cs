using System.Collections.Generic;
using FantasyAggregatorApp.Models;
using FantasyAggregatorApp.Repositories;

namespace FantasyAggregator.Business.Services
{
    public class PlatformService
    {
        private readonly PlatformRepository _repo = new PlatformRepository();

        public IEnumerable<Platform> GetAll() => _repo.GetAll();
        public Platform Get(int id) => _repo.GetById(id);
        public int Create(Platform p) => _repo.Create(p);
        public bool Update(Platform p) => _repo.Update(p);
        public bool Delete(int id) => _repo.Delete(id);
    }
}
