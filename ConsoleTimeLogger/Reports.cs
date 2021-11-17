using System;
using Microsoft.Data.Sqlite;
using ConsoleTableExt;
using System.Collections.Generic;

namespace ConsoleTimeLogger
{
    class Reports
    {
        private SqliteConnection Connection;
        public Reports(string fileName)
        {
            this.Connection = new SqliteConnection($"Data Source={fileName}");
            this.Connection.Open();
        }
        public void PickReports()
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n These reports will show total hours for the given dates");
            Console.WriteLine("\n Which report? \n");
            Console.WriteLine("R to start from X days ago");
            Console.WriteLine("M to see the total of a given month");
            Console.WriteLine("S to enter a specific starting date up to Today");
            Console.WriteLine("B to choose a specific start and end date");
            Console.WriteLine("A to see total of all time");
            Console.WriteLine("0 to return to main menu");
            Console.WriteLine("-------------------------------------");

            string userInput = Console.ReadLine().ToUpper();
            switch (userInput)
            {
                case "R":
                    bool correct = User.getUserXDays(out double result);
                    if (correct)
                    {
                        Console.Clear();
                        ReportXDays(result);
                    }
                    UserInputWait();
                    PickReports();
                    break;
                case "S":
                    Console.Clear();
                    long startDate = User.DateInput();
                    if (startDate != -1)
                    {
                        Console.Clear();
                        ReportFromDate(startDate);
                    }
                    UserInputWait();
                    PickReports();
                    break;
                case "A":
                    Console.Clear();
                    ReportAll();
                    UserInputWait();
                    PickReports();
                    break;
                case "B":
                    Console.Clear();
                    Console.WriteLine("Input the starting date");
                    long firstDate = User.DateInput();
                    if (firstDate != -1)
                    {
                        Console.Clear();
                        Console.WriteLine("Input the end date");
                        long secondDate = User.DateInput();
                        if (secondDate != -1)
                        {
                            ReportFromDate(firstDate, secondDate);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Incorrect date input, returning to main menu");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Incorrect date input, returning to main menu");
                    }
                    UserInputWait();
                    PickReports();
                    break;
                case "M":
                    Console.Clear();
                    bool getUserMonthInput = User.GetUserMonthInput(out int monthChoice);
                    bool getUserYearInput = User.GetUserYearInput(out int yearChoice);
                    if (getUserMonthInput && getUserYearInput)
                    {
                        ReportMonth(monthChoice, yearChoice);
                    }
                    UserInputWait();
                    PickReports();
                    break;
                case "0":
                    Console.Clear();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Un-recognized input. Press any key to return to reports menu");
                    Console.ReadLine();
                    PickReports();
                    break;
            }

        }
        private static void DisplayReportResults(List<List<object>> tableData)
        {
            ConsoleTableBuilder
               .From(tableData)
               .WithFormat(ConsoleTableBuilderFormat.Alternative)
               .ExportAndWriteLine(TableAligntment.Left);
        }
        private void ReportMonth(int month, int year)
        {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = (firstDayOfMonth.AddMonths(1)).AddDays(-1);
            Console.Clear();
            this.ReportFromDate(firstDayOfMonth.Ticks, lastDayOfMonth.Ticks);
        }

        private void ReportXDays(double xDays)
        {
            long startDate = SubtractXDaysFromToday(xDays);
            long today = DatabaseManager.GetTodayDate();
            long totalHours = 0;
            var tableData = new List<List<object>> { new List<object> { "Start Date", "End Date", "Total Hours" } };

            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {today} AND date >= {startDate}";

            using var reader = selectCmd.ExecuteReader();
            
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                totalHours += long.Parse(hours);
            }

            tableData.Add(new List<object> { DatabaseManager.ParseDate(startDate.ToString()), 
                DatabaseManager.ParseDate(today.ToString()), ParseHours(totalHours) });
            
            DisplayReportResults(tableData);
        }
        private static long SubtractXDaysFromToday(double xDays)
        {
            DateTime today = new DateTime(DatabaseManager.GetTodayDate());
            xDays *= -1;
            DateTime XfromToday = today.AddDays(xDays);
            return XfromToday.Ticks;
        }

        private void ReportAll()
        {
            long totalHours = 0;
            var tableData = new List<List<object>> { new List<object> { "Start Date", "End Date", "Total Hours" } };

            var firstDatecmd = this.Connection.CreateCommand();
            firstDatecmd.CommandText = $"SELECT MIN(date) from 'time'";
            long firstDate = (long)firstDatecmd.ExecuteScalar();

            var lastDatecmd = this.Connection.CreateCommand();
            lastDatecmd.CommandText = $"SELECT MAX(date) FROM 'time'";
            long lastDate = (long)lastDatecmd.ExecuteScalar();

            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time";
            using var reader = selectCmd.ExecuteReader();
            
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                totalHours += long.Parse(hours);
            }

            tableData.Add(new List<object> { DatabaseManager.ParseDate(firstDate.ToString()),
                DatabaseManager.ParseDate(lastDate.ToString()), ParseHours(totalHours) });
            
            DisplayReportResults(tableData);
        }

        private void ReportFromDate(long startDate)
        {
            long today = DatabaseManager.GetTodayDate();
            var tableData = new List<List<object>> { new List<object> { "Start Date", "End Date", "Total Hours" } };
            long totalHours = 0;

            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {today} AND date >= {startDate}";
            using var reader = selectCmd.ExecuteReader();
            
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                totalHours += long.Parse(hours);
            }

            tableData.Add(new List<object> { DatabaseManager.ParseDate(startDate.ToString()),
                DatabaseManager.ParseDate(today.ToString()), ParseHours(totalHours) });
            
            DisplayReportResults(tableData);
        }
        private void ReportFromDate(long startDate, long endDate)
        {
            long totalHours = 0;
            var tableData = new List<List<object>> { new List<object> { "Start Date", "End Date", "Total Hours" } };

            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {endDate} AND date >= {startDate}";
            using var reader = selectCmd.ExecuteReader();
            
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                totalHours += long.Parse(hours);
            }

            tableData.Add(new List<object> { DatabaseManager.ParseDate(startDate.ToString()),
                DatabaseManager.ParseDate(endDate.ToString()), ParseHours(totalHours) });
            
            DisplayReportResults(tableData);
        }
        
        private static void UserInputWait()
        {
            Console.WriteLine("\n Press any key to return to the menu");
            Console.ReadLine();
            Console.Clear();
        }

        public static string ParseHours(long hours)
        {
            TimeSpan ts = new TimeSpan(hours);
            
            return string.Format("{0}hr {1}mn {2}sec",
                     (int)ts.TotalHours,
                     ts.Minutes,
                     ts.Seconds);
        }
    }
}
