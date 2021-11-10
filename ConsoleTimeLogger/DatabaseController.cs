using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    //no user input should be taken in this file, it should be passed in from elsewhere
    class DatabaseController
    {
        public class Edit
        {
            public static void InsertHours(SqliteConnection connection)
            {
                Console.WriteLine("Input how many hours you would like to insert");
                string numberInput = Console.ReadLine();
                int hours = Convert.ToInt16(numberInput);
                if (hours > 0)
                {
                    var transaction = connection.BeginTransaction();
                    var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hours}, {GetDay()})";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine($"Logged {hours} hours for {GetDay()}");
                }
            }

            public static void AddHours(SqliteConnection connection, int addHours, string day = null)
            {
                if (day == null)
                {
                    day = GetDay();
                }

                if (addHours > 0)
                {

                    try
                    {
                        var transaction = connection.BeginTransaction();
                        var updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = $"UPDATE time SET hours=hours+{addHours} WHERE date = {day}";
                        updateCmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        Console.WriteLine("Are you sure you inputted the correct ID?");
                    }
                }
            }
        }

        public class View
        {
            public static void DBSearch(SqliteConnection connection, string day=null)
            {
                if (day == null)
                {
                    day = GetDay();
                }

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

            public static void ViewAll(SqliteConnection connection)
            {
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT * FROM time";
                using var reader = selectCmd.ExecuteReader();
                while (reader.Read())
                {
                    var key = reader.GetString(0);
                    var hours = reader.GetInt32(1);
                    var date = reader.GetString(2);
                    Console.WriteLine($"KeyID: {key}, Hours: {hours}, Date: {date}");
                }
            }
        }

        public static string GetDay()
        {
            DateTime today = DateTime.Today;
            return today.ToString("yyyyMMdd");
        }

        public class Delete
        {
            public static void DeleteItem(SqliteConnection connection, string deleteID)
            {
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
        }

        

        

        public static void UpdateItem(SqliteConnection connection, int hoursUpdate, string updateDate)
        {
            if (hoursUpdate >= 0)
            {
                try
                {
                    var transaction = connection.BeginTransaction();
                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = $"UPDATE time SET hours={hoursUpdate} WHERE date = {updateDate}";
                    updateCmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    Console.WriteLine("Are you sure you inputted the correct ID?");
                }
            }
        }

    }
}
