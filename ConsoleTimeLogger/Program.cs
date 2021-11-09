using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class Program
    {
        public static string getDay()
        {
            DateTime today = DateTime.Today;
            return today.ToString("ddMMyyyy");
        }
        static void InsertHours(int hours, Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            if(hours > 0)
            {
                var transaction = connection.BeginTransaction();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hours}, {getDay()})";
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }
        static void Main(string[] args)
        {
            using var connection = new SqliteConnection("Data Source=time.db");
            connection.Open();
            
            Console.WriteLine(getDay());

            //try to create a table
            try
            {
                var createTable = connection.CreateCommand();
                createTable.CommandText = @"CREATE TABLE time(hours INT, 
                                                          date DATE
                                                                    );";
                createTable.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table already exists");
            }

            int userInput = 1;
            while (userInput != 0)
            {
                Console.WriteLine("Input your command");
                Console.WriteLine("0 to exit");
                Console.WriteLine("Insert any number to add that many hours to todays coding time");
                string newInput = Console.ReadLine();
                userInput = Convert.ToInt16(newInput);
                InsertHours(userInput, connection);
                
            }


            //insert records
            
            /*
            var transaction = connection.BeginTransaction();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES('1', {getDay()})";
            insertCmd.ExecuteNonQuery();

            transaction.Commit();
            */

            //read records
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time";
            using var reader = selectCmd.ExecuteReader(); 
            while (reader.Read())
            {
                var first = reader.GetString(0);
                var second = reader.GetString(1);
                Console.WriteLine(first);
                Console.WriteLine(second);
            }
        }
    }
}
