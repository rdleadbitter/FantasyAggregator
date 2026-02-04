using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FantasyAggregatorApp.Data;
using FantasyAggregatorApp.Models;

namespace FantasyAggregatorApp.Repositories
{
    public class PlatformRepository
    {
        public IEnumerable<Platform> GetAll()
        {
            var list = new List<Platform>();
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT PlatformId, Name, Url FROM Platforms", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Platform {
                    PlatformId = rdr.GetInt32("PlatformId"),
                    Name = rdr.GetString("Name"),
                    Url = rdr.IsDBNull(rdr.GetOrdinal("Url")) ? null : rdr.GetString("Url")
                });
            }
            return list;
        }

        public Platform GetById(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT PlatformId, Name, Url FROM Platforms WHERE PlatformId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Platform {
                    PlatformId = rdr.GetInt32("PlatformId"),
                    Name = rdr.GetString("Name"),
                    Url = rdr.IsDBNull(rdr.GetOrdinal("Url")) ? null : rdr.GetString("Url")
                };
            }
            return null;
        }

        public int Create(Platform p)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO Platforms (Name, Url) VALUES (@name, @url); SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@name", p.Name);
            cmd.Parameters.AddWithValue("@url", (object)p.Url ?? DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public bool Update(Platform p)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE Platforms SET Name=@name, Url=@url WHERE PlatformId=@id", conn);
            cmd.Parameters.AddWithValue("@name", p.Name);
            cmd.Parameters.AddWithValue("@url", (object)p.Url ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", p.PlatformId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbConnector.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Platforms WHERE PlatformId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
