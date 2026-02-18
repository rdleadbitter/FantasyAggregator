using System.Collections.Generic;
using FantasyAggregatorApp.Models;
using FantasyAggregatorApp.Repositories;

namespace FantasyAggregator.Business.Services
{
    /// <summary>
    /// Business-layer service for users.
    /// </summary>
    public class UserService
    {
        private readonly UserRepository _repo = new UserRepository();

        public IEnumerable<User> GetAllUsers() => _repo.GetAll();
        public User GetUser(int id) => _repo.GetById(id);
        public int CreateUser(User u) => _repo.Create(u);
        public bool UpdateUser(User u) => _repo.Update(u);
        public bool DeleteUser(int id) => _repo.Delete(id);
    }
}
