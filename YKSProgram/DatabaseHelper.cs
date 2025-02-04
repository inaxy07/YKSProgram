using System;
using System.Data.SQLite;

namespace YKSProgram
{
    public class DatabaseHelper
    {
        private readonly string connectionString = "Data Source=UserDatabase.db;Version=3;";

        public SQLiteConnection CreateConnection()
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        public void ExecuteQuery(string query, SQLiteParameter[] parameters = null)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetPragmaSettings()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("PRAGMA busy_timeout = 5000; PRAGMA journal_mode = WAL;", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        public SQLiteDataReader ExecuteReader(string query, SQLiteParameter[] parameters = null)
        {
            var connection = CreateConnection();
            var command = new SQLiteCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public SQLiteConnection GetConnection()
        {
            string connectionString = "Data Source=UserDatabase.db;Version=3;";
            return new SQLiteConnection(connectionString);
        }
    }
}
