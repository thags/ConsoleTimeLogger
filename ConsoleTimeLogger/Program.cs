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


            //try to create a table
            try
            {
                var createTable = connection.CreateCommand();
                createTable.CommandText = "CREATE TABLE time(name VARCHAR(50));";
                createTable.ExecuteNonQuery();
            } 
            //if the table is already there it will notify of that, just for testing purposes, but will continue on to the rest of the code. 
            catch
            {
                Console.WriteLine("table already exists, continuing...");
            }
            

            //insert records
            var transaction = connection.BeginTransaction();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO time VALUES('test')";
            insertCmd.ExecuteNonQuery();

            transaction.Commit();

            //read records
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM time";
            using (var reader = selectCmd.ExecuteReader()) {
                while (reader.Read())
                {
                    var result = reader.GetString(0);
                    Console.WriteLine(result);
                }
            }
        }
    }
}
