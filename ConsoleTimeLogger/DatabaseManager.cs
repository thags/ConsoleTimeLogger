using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class DatabaseManager
    {

        public static void CreateTableIfNonExistent(SqliteConnection connection)
        {
            //try to create a table
            try
            {
                var createTable = connection.CreateCommand();
                createTable.CommandText = @"CREATE TABLE time(id INTEGER PRIMARY KEY,
                                                                hours INTEGER, 
                                                                date STRING
                                                                    );";
                createTable.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table already exists");
            }
        }
    }
}
