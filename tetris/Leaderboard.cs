using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace tetris
{
    class Leaderboard
    {
        private readonly SqliteConnection connection = new("Data Source=leaderboard.db");
        public Leaderboard()
        {
            connection.Open();
            SqliteCommand command = new("CREATE TABLE IF NOT EXISTS Leaders(id INTEGER NOT NULL PRIMARY KEY " +
                    "AUTOINCREMENT UNIQUE, score INTEGER NOT NULL, name TEXT NOT NULL)", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddToDb(int score, string nick)
        {
            connection.Open();
            SqliteCommand command = new($"INSERT INTO Leaders (score, name) VALUES({score}, '{nick}')", connection);
            command.ExecuteNonQuery();
            connection.Close(); 
            
        }

        public List<Leader> GetLeaderboard()
        {
            List<Leader> leaders = new();

            connection.Open();
            SqliteCommand command = new("SELECT name, score FROM Leaders ORDER BY score DESC LIMIT 10", connection);


            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int score = reader.GetInt32(1);
                        Leader temp = new(name, score);
                        leaders.Add(temp);
                    }
                }
            }
            connection.Close();
            return leaders;
        }
    }
}
