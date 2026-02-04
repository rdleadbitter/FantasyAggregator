using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Models;

namespace FantasyAggregatorApp.Repositories
{
    public class TeamRepository
    {
        public IEnumerable<Team> GetAll()
        {
            var list = new List<Team>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT TeamId, UserId, PlatformId, PlatformTeamId, TeamName, LeagueName FROM Teams", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Team {
                    TeamId = rdr.GetInt32("TeamId"),
                    UserId = rdr.GetInt32("UserId"),
                    PlatformId = rdr.GetInt32("PlatformId"),
                    PlatformTeamId = rdr.GetString("PlatformTeamId"),
                    TeamName = rdr.GetString("TeamName"),
                    LeagueName = rdr.IsDBNull(rdr.GetOrdinal("LeagueName")) ? null : rdr.GetString("LeagueName")
                });
            }
            return list;
        }

        public Team GetById(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT TeamId, UserId, PlatformId, PlatformTeamId, TeamName, LeagueName FROM Teams WHERE TeamId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Team {
                    TeamId = rdr.GetInt32("TeamId"),
                    UserId = rdr.GetInt32("UserId"),
                    PlatformId = rdr.GetInt32("PlatformId"),
                    PlatformTeamId = rdr.GetString("PlatformTeamId"),
                    TeamName = rdr.GetString("TeamName"),
                    LeagueName = rdr.IsDBNull(rdr.GetOrdinal("LeagueName")) ? null : rdr.GetString("LeagueName")
                };
            }
            return null;
        }

        public int Create(Team t)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO Teams (UserId, PlatformId, PlatformTeamId, TeamName, LeagueName) VALUES (@u,@p,@ptid,@name,@league); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@u", t.UserId);
            cmd.Parameters.AddWithValue("@p", t.PlatformId);
            cmd.Parameters.AddWithValue("@ptid", t.PlatformTeamId);
            cmd.Parameters.AddWithValue("@name", t.TeamName);
            cmd.Parameters.AddWithValue("@league", (object)t.LeagueName ?? DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public bool Update(Team t)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE Teams SET UserId=@u, PlatformId=@p, PlatformTeamId=@ptid, TeamName=@name, LeagueName=@league WHERE TeamId=@id", conn);
            cmd.Parameters.AddWithValue("@u", t.UserId);
            cmd.Parameters.AddWithValue("@p", t.PlatformId);
            cmd.Parameters.AddWithValue("@ptid", t.PlatformTeamId);
            cmd.Parameters.AddWithValue("@name", t.TeamName);
            cmd.Parameters.AddWithValue("@league", (object)t.LeagueName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", t.TeamId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Teams WHERE TeamId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
