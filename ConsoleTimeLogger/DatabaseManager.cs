using System;
using Microsoft.Data.Sqlite;
using ConsoleTableExt;
using System.Collections.Generic;

namespace ConsoleTimeLogger
{
    class DatabaseManager
    {
        readonly private SqliteConnection Connection;
        public DatabaseManager(string fileName)
        {
            this.Connection = new SqliteConnection($"Data Source={fileName}");
            this.Connection.Open();
            this.CreateTableIfNonExistent();
        }
        public static long GetTodayDate() => DateTime.Today.Ticks;

        public void CreateTableIfNonExistent()
        {
            //we want to make sure that the DB has the proper table and columns
            try
            {
                var createTable = this.Connection.CreateCommand();
                createTable.CommandText = @"CREATE TABLE time(id INTEGER PRIMARY KEY,
                                                                hours LONG, 
                                                                date LONG
                                                                    );";
                createTable.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Table already exists");
            }
        }

        public void View(string selection=null, int limit = 1, long specific = -1)
        {
            var tableData = new List<List<object>> { new List<object> { "Date", "Hours" } };
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM time";

            switch (selection)
            {
                case null:
                    Console.WriteLine("Todays Entry: ");
                    selectCmd.CommandText = $"SELECT * FROM time WHERE date={GetTodayDate()}";
                    break;
                case "all":
                    selectCmd.CommandText = $"SELECT * FROM time ORDER BY date DESC";
                    break;
                case "limit":
                    selectCmd.CommandText = $"SELECT * FROM time ORDER BY date DESC LIMIT {limit}";
                    break;
                case "specific":
                    selectCmd.CommandText = $"SELECT * FROM time WHERE date={specific}";
                    break;
                default:
                    //selectCmd.CommandText = $"SELECT * FROM time WHERE date={day}";
                    break;
            }

            using var reader = selectCmd.ExecuteReader();
            
            while (reader.Read())
            {
                
                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                tableData.Add(new List<object>{ParseDate(date), ParseHours(hours)});
            }

            ConsoleTableBuilder
                .From(tableData)
                .WithFormat(ConsoleTableBuilderFormat.Alternative)
                .ExportAndWriteLine(TableAligntment.Left);
        }
        public void Update(long addHours, long day)
        {
            try
            {
                var transaction = this.Connection.BeginTransaction();
                var updateCmd = this.Connection.CreateCommand();

                updateCmd.CommandText = $"UPDATE time SET hours={addHours} WHERE date = {day}";
                updateCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct date?");
            }
        }

        public void Delete(long day)
        {
            try
            {
                var transaction = this.Connection.BeginTransaction();
                var deleteCmd = this.Connection.CreateCommand();
                deleteCmd.CommandText = $"DELETE FROM time WHERE date = {day}";
                deleteCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct Date?");
            }
        }

        public void InsertRow(long day, long hoursInput)
        {
            bool rowDoesntExist = CheckRowDateDoesntExist(day);

            if (rowDoesntExist)
            {
                var transaction = this.Connection.BeginTransaction();
                var insertCmd = this.Connection.CreateCommand();

                insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hoursInput},{day})";
                insertCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            else
            {
                Console.Clear();
                this.View("specific", specific:day);

                Console.WriteLine($"Your entered date of {ParseDate(day.ToString())} already exists");
                Console.WriteLine("U to update this days entry to the time provided");
                Console.WriteLine("0 or any other input to return to the main menu with no changes");

                string choice = Console.ReadLine().ToUpper();
                switch (choice)
                {
                    case "U":
                        Update(hoursInput, day);
                        View("specific", specific:day);

                        Console.WriteLine("Entry has been updated");
                        Console.WriteLine("Press any key to return to main menu");
                        Console.ReadLine();
                        break;
                    default:
                        break;
                }
                Console.Clear();
            }
            
        }
        private bool CheckRowDateDoesntExist(long desiredInput)
        {
            var checkCmd = this.Connection.CreateCommand();
            checkCmd.CommandText = $"SELECT COUNT(*) FROM time WHERE date = {desiredInput}";
            long result = (long)checkCmd.ExecuteScalar();
            if (result > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static string ParseDate(string day)
        {
            DateTime dt = new DateTime(long.Parse(day));
            return dt.ToString("MM-dd-yyyy");
        }
        public static string ParseHours(string hours)
        {
            TimeSpan ts = new TimeSpan(long.Parse(hours));
            return ts.ToString("h\\:mm");
        }
    }
}
