using System;
using MySql.Data.MySqlClient;

namespace FantasyAggregatorApp.Data
{
    public static class DbConnector
    {
        private static string _connectionString;

        public static void Init(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string not initialized. Call DbConnector.Init(...) first.");
            var conn = new MySqlConnection(_connectionString);
            return conn;
        }
    }
}
