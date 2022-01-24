using System;
using Microsoft.Data.Sqlite;
using ConsoleTableExt;
using System.Collections.Generic;

namespace ConsoleTimeLogger
{
    public class DatabaseManager
    {
        readonly private string Filename;
        public DatabaseManager(string fileName)
        {
            this.Filename = fileName;
            this.CreateTableIfNonExistent();
        }
        public static long GetTodayDate() => DateTime.Today.Ticks;

        public void CreateTableIfNonExistent()
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"CREATE TABLE time(id INTEGER PRIMARY KEY,
                                                                hours LONG, 
                                                                date LONG
                                                                    );";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                    }
                } 
            }   
        }

        public void View(string selection=null, int limit = 1, long specific = -1)
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    var tableData = new List<List<object>> { new List<object> { "Date", "Hours" } };
                    command.CommandText = "SELECT * FROM time";

                    switch (selection)
                    {
                        case null:
                            Console.WriteLine("Todays Entry: ");
                            command.CommandText = $"SELECT * FROM time WHERE date={GetTodayDate()}";
                            break;
                        case "all":
                            command.CommandText = $"SELECT * FROM time ORDER BY date DESC";
                            break;
                        case "limit":
                            command.CommandText = $"SELECT * FROM time ORDER BY date DESC LIMIT {limit}";
                            break;
                        case "specific":
                            command.CommandText = $"SELECT * FROM time WHERE date={specific}";
                            break;
                        default:
                            //command.CommandText = $"SELECT * FROM time WHERE date={day}";
                            break;
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            var hours = reader.GetString(1);
                            var date = reader.GetString(2);
                            tableData.Add(new List<object> { ParseDate(date), ParseHours(hours) });
                        }

                        ConsoleTableBuilder
                            .From(tableData)
                            .WithFormat(ConsoleTableBuilderFormat.Alternative)
                            .ExportAndWriteLine(TableAligntment.Left);
                    }
                }
            }
            
        }
        public void Update(long addHours, long day)
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"UPDATE time SET hours={addHours} WHERE date = {day}";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                    }
                }
            }
        }

        public void Delete(long day)
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"DELETE FROM time WHERE date = {day}";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                    }
                }
                
            }
        }

        public void InsertRow(long day, long hoursInput)
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO time(hours, date) VALUES({hoursInput},{day})";

                    try
                    {
                        bool rowDoesntExist = CheckRowDateDoesntExist(day);
                        if (rowDoesntExist)
                        {
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            Console.Clear();
                            this.View("specific", specific: day);

                            Console.WriteLine($"Your entered date of {ParseDate(day.ToString())} already exists");
                            Console.WriteLine("U to update this days entry to the time provided");
                            Console.WriteLine("0 or any other input to return to the main menu with no changes");

                            string choice = Console.ReadLine().ToUpper();
                            switch (choice)
                            {
                                case "U":
                                    Update(hoursInput, day);
                                    View("specific", specific: day);

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
                    catch (SqliteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                    }
                }
            }
            
        }
        private bool CheckRowDateDoesntExist(long desiredInput)
        {
            using (var connection = new SqliteConnection($"Data Source={this.Filename}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT COUNT(*) FROM time WHERE date = {desiredInput}";
                    try
                    {
                        long result = (long)command.ExecuteScalar();
                        if (result > 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (SqliteException e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Dispose();
                    }
                }
                
            }
        }
        public static string ParseDate(string day)
        {
            try
            {
                DateTime dt = new DateTime(long.Parse(day));
                return dt.ToString("MM-dd-yyyy");
            }
            catch (FormatException e)
            {
                return e.Message;
            }
            
        }
        public static string ParseHours(string hours)
        {
            try
            {
                TimeSpan ts = new TimeSpan(long.Parse(hours));
                return ts.ToString("hh\\:mm");
            }
            catch (FormatException e)
            {
                return e.Message;
            }
        }
    }
}
