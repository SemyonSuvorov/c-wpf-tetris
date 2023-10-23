using Microsoft.Data.Sqlite;

namespace tetris
{
    class Leaderboard
    {
        private SqliteConnection connection = new SqliteConnection("Data Source=leaderboard.db");
        public Leaderboard()
        {
            using (connection)
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Leaders(id INTEGER NOT NULL PRIMARY KEY " +
                    "AUTOINCREMENT UNIQUE, score INTEGER NOT NULL, name TEXT NOT NULL)", connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddToDb(int score, string nick)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand($"INSERT INTO Leaders (score, name) VALUES({score}, '{nick}')", connection);
            command.ExecuteNonQuery();
        }
    }
}
