using System.Collections.Generic;
using FantasyAggregatorApp.Models;
using FantasyAggregatorApp.Repositories;

namespace FantasyAggregator.Business.Services
{
    /// <summary>
    /// Business-layer service for player operations.
    /// Wraps PlayerRepository (data layer) and provides methods used by the API layer.
    /// </summary>
    public class PlayerService
    {
        private readonly PlayerRepository _repo = new PlayerRepository();

        public IEnumerable<Player> GetAllPlayers() => _repo.GetAll();

        public Player GetPlayerById(int id) => _repo.GetById(id);

        public int CreatePlayer(Player p) => _repo.Create(p);

        public bool UpdatePlayer(Player p) => _repo.Update(p);

        public bool DeletePlayer(int id) => _repo.Delete(id);

        // Extra convenience function for search
        public IEnumerable<Player> SearchByName(string partialName)
        {
            var all = _repo.GetAll();
            if (string.IsNullOrWhiteSpace(partialName)) return all;
            return System.Linq.Enumerable.Where(all, p =>
                p.FullName != null && p.FullName.IndexOf(partialName, System.StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
