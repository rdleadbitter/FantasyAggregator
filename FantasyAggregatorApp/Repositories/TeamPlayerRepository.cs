using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Models;

namespace FantasyAggregatorApp.Repositories
{
    public class TeamPlayerRepository
    {
        public IEnumerable<TeamPlayer> GetAll()
        {
            var list = new List<TeamPlayer>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT TeamPlayerId, TeamId, PlayerId, RosterSlot, AcquiredOn FROM TeamPlayers", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new TeamPlayer {
                    TeamPlayerId = rdr.GetInt32("TeamPlayerId"),
                    TeamId = rdr.GetInt32("TeamId"),
                    PlayerId = rdr.GetInt32("PlayerId"),
                    RosterSlot = rdr.IsDBNull(rdr.GetOrdinal("RosterSlot")) ? null : rdr.GetString("RosterSlot"),
                    AcquiredOn = rdr.IsDBNull(rdr.GetOrdinal("AcquiredOn")) ? (DateTime?)null : rdr.GetDateTime("AcquiredOn")
                });
            }
            return list;
        }

        public TeamPlayer GetById(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT TeamPlayerId, TeamId, PlayerId, RosterSlot, AcquiredOn FROM TeamPlayers WHERE TeamPlayerId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new TeamPlayer {
                    TeamPlayerId = rdr.GetInt32("TeamPlayerId"),
                    TeamId = rdr.GetInt32("TeamId"),
                    PlayerId = rdr.GetInt32("PlayerId"),
                    RosterSlot = rdr.IsDBNull(rdr.GetOrdinal("RosterSlot")) ? null : rdr.GetString("RosterSlot"),
                    AcquiredOn = rdr.IsDBNull(rdr.GetOrdinal("AcquiredOn")) ? (DateTime?)null : rdr.GetDateTime("AcquiredOn")
                };
            }
            return null;
        }

        public int Create(TeamPlayer tp)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO TeamPlayers (TeamId, PlayerId, RosterSlot, AcquiredOn) VALUES (@teamId, @playerId, @slot, @acq); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@teamId", tp.TeamId);
            cmd.Parameters.AddWithValue("@playerId", tp.PlayerId);
            cmd.Parameters.AddWithValue("@slot", (object)tp.RosterSlot ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@acq", tp.AcquiredOn.HasValue ? (object)tp.AcquiredOn.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public bool Update(TeamPlayer tp)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE TeamPlayers SET TeamId=@teamId, PlayerId=@playerId, RosterSlot=@slot, AcquiredOn=@acq WHERE TeamPlayerId=@id", conn);
            cmd.Parameters.AddWithValue("@teamId", tp.TeamId);
            cmd.Parameters.AddWithValue("@playerId", tp.PlayerId);
            cmd.Parameters.AddWithValue("@slot", (object)tp.RosterSlot ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@acq", tp.AcquiredOn.HasValue ? (object)tp.AcquiredOn.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.Parameters.AddWithValue("@id", tp.TeamPlayerId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM TeamPlayers WHERE TeamPlayerId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        // Example helper: get roster for a team with player names (simple join)
        public IEnumerable<(TeamPlayer, string playerName)> GetRosterWithNames(int teamId)
        {
            var list = new List<(TeamPlayer, string)>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                @"SELECT tp.TeamPlayerId, tp.TeamId, tp.PlayerId, tp.RosterSlot, tp.AcquiredOn, p.FullName
                  FROM TeamPlayers tp
                  JOIN Players p ON tp.PlayerId = p.PlayerId
                  WHERE tp.TeamId = @teamId
                  ORDER BY tp.RosterSlot", conn);
            cmd.Parameters.AddWithValue("@teamId", teamId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var tp = new TeamPlayer {
                    TeamPlayerId = rdr.GetInt32("TeamPlayerId"),
                    TeamId = rdr.GetInt32("TeamId"),
                    PlayerId = rdr.GetInt32("PlayerId"),
                    RosterSlot = rdr.IsDBNull(rdr.GetOrdinal("RosterSlot")) ? null : rdr.GetString("RosterSlot"),
                    AcquiredOn = rdr.IsDBNull(rdr.GetOrdinal("AcquiredOn")) ? (DateTime?)null : rdr.GetDateTime("AcquiredOn")
                };
                var name = rdr.IsDBNull(rdr.GetOrdinal("FullName")) ? null : rdr.GetString("FullName");
                list.Add((tp, name));
            }
            return list;
        }
    }
}
