using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Models;

namespace FantasyAggregatorApp.Repositories
{
    public class UserRepository
    {
        public IEnumerable<User> GetAll()
        {
            var list = new List<User>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT UserId, Username, Email, CreatedAt FROM Users", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new User {
                    UserId = rdr.GetInt32("UserId"),
                    Username = rdr.GetString("Username"),
                    Email = rdr.GetString("Email"),
                    CreatedAt = rdr.GetDateTime("CreatedAt")
                });
            }
            return list;
        }

        public User GetById(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT UserId, Username, Email, CreatedAt FROM Users WHERE UserId = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new User {
                    UserId = rdr.GetInt32("UserId"),
                    Username = rdr.GetString("Username"),
                    Email = rdr.GetString("Email"),
                    CreatedAt = rdr.GetDateTime("CreatedAt")
                };
            }
            return null;
        }

        public int Create(User u)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO Users (Username, Email) VALUES (@username, @email); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@username", u.Username);
            cmd.Parameters.AddWithValue("@email", u.Email);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public bool Update(User u)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE Users SET Username=@username, Email=@email WHERE UserId=@id", conn);
            cmd.Parameters.AddWithValue("@username", u.Username);
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@id", u.UserId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Users WHERE UserId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
