using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class Program
    {
        public static string getDay()
        {
            DateTime today = DateTime.Today;
            return today.ToString("yyyyMMdd");
        }
        static void InsertHours(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            Console.WriteLine("Input how many hours you would like to insert");
            string numberInput = Console.ReadLine();
            int hours = Convert.ToInt16(numberInput);
            if (hours > 0)
            {
                var transaction = connection.BeginTransaction();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hours}, {getDay()})";
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine($"Logged {hours} hours for {getDay()}");
            }
        }
        static void DBSearch(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            Console.WriteLine("Input the day you want to search for in yyyyMMdd format");
            string day = Console.ReadLine();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date = {day}";
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var key = reader.GetString(0);
                var hours = reader.GetInt32(1);
                var date = reader.GetString(2);
                Console.WriteLine($"KeyID: {key}, Hours: {hours}, Date: {date}");
            }
        }
        static void DeleteItem(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            Console.WriteLine("Input the ID of the item you wish to delete");
            string deleteID = Console.ReadLine();
            try
            {
                var transaction = connection.BeginTransaction();
                var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = $"DELETE FROM time WHERE id = {deleteID}";
                deleteCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct ID?");
            }
            
        }
        static void Main(string[] args)
        {
            using var connection = new SqliteConnection("Data Source=time.db");
            connection.Open();

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

            //loop
            int userInput = 1;
            while (userInput != 0)
            {
                Console.WriteLine("Input your command");
                Console.WriteLine("0 to exit, I to insert hours, S to search, D to delete");
                string newInput = Console.ReadLine().ToUpper();
                Console.WriteLine("-------------------------------------");
                if (newInput == "0" || newInput == "O")
                {
                    userInput = 0;
                }
                else if(newInput == "I")
                {
                    InsertHours(connection);
                }
                else if(newInput == "S")
                {
                    DBSearch(connection);
                    
                }
                else if(newInput == "D")
                {
                    DeleteItem(connection);
                }
                Console.WriteLine("-------------------------------------");


            }

            
        }
    }
}
