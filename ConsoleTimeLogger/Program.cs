using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new SqliteConnection("Data Source=time.db");
            connection.Open();

            DatabaseManager.CreateTableIfNonExistent(connection);

            UserInput.InputLoop(connection);

        }
    }
}
