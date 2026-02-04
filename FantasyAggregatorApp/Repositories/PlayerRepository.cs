using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Models;

namespace FantasyAggregatorApp.Repositories
{
    public class PlayerRepository
    {
        public IEnumerable<Player> GetAll()
        {
            var list = new List<Player>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT PlayerId, FullName, Position, TeamAbbrev, Active FROM Players", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Player {
                    PlayerId = rdr.GetInt32("PlayerId"),
                    FullName = rdr.GetString("FullName"),
                    Position = rdr.IsDBNull(rdr.GetOrdinal("Position")) ? null : rdr.GetString("Position"),
                    TeamAbbrev = rdr.IsDBNull(rdr.GetOrdinal("TeamAbbrev")) ? null : rdr.GetString("TeamAbbrev"),
                    Active = rdr.GetBoolean("Active")
                });
            }
            return list;
        }

        public Player GetById(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT PlayerId, FullName, Position, TeamAbbrev, Active FROM Players WHERE PlayerId = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Player {
                    PlayerId = rdr.GetInt32("PlayerId"),
                    FullName = rdr.GetString("FullName"),
                    Position = rdr.IsDBNull(rdr.GetOrdinal("Position")) ? null : rdr.GetString("Position"),
                    TeamAbbrev = rdr.IsDBNull(rdr.GetOrdinal("TeamAbbrev")) ? null : rdr.GetString("TeamAbbrev"),
                    Active = rdr.GetBoolean("Active")
                };
            }
            return null;
        }

        public int Create(Player p)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO Players (FullName, Position, TeamAbbrev, Active) VALUES (@name, @pos, @ta, @active); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@name", p.FullName);
            cmd.Parameters.AddWithValue("@pos", (object)p.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ta", (object)p.TeamAbbrev ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@active", p.Active);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public bool Update(Player p)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE Players SET FullName=@name, Position=@pos, TeamAbbrev=@ta, Active=@active WHERE PlayerId=@id", conn);
            cmd.Parameters.AddWithValue("@name", p.FullName);
            cmd.Parameters.AddWithValue("@pos", (object)p.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ta", (object)p.TeamAbbrev ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@active", p.Active);
            cmd.Parameters.AddWithValue("@id", p.PlayerId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Players WHERE PlayerId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
