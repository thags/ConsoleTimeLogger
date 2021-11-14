using System;
using Microsoft.Data.Sqlite;
using ConsoleTableExt;
using System.Collections.Generic;
using System.Globalization;

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
            Console.WriteLine("Which report?");
            Console.WriteLine("R to choose how many days ago to start from to Today");
            Console.WriteLine("S to enter a specific date to start from to Today");
            Console.WriteLine("A to see total of all days hours");
            Console.WriteLine("B to choose a specific start and end date");
            Console.WriteLine("M to choose total time of a specific month");
            Console.WriteLine("0 to return to main menu");

            string userInput = Console.ReadLine().ToUpper();
            switch (userInput)
            {
                case "R":
                    bool correct = getUserInt(out double result);
                    if (correct)
                    {
                        Console.Clear();
                        ReportXDays(result);
                    }
                    UserInputWait();
                    break;
                case "S":
                    long startDate = User.DateInput();
                    if (startDate != -1)
                    {
                        Console.Clear();
                        ReportFromDate(startDate);
                    }
                    UserInputWait();
                    break;
                case "A":
                    Console.Clear();
                    ReportAll();
                    UserInputWait();
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
                    break;
                case "M":
                    Console.Clear();
                    bool userMonthInput = GetUserMonthInput(out int monthChoice);
                    if (userMonthInput)
                    {
                        ReportMonth(monthChoice);
                    }
                    UserInputWait();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Un-recognized input. Press any key to return to reports menu");
                    Console.ReadLine();
                    PickReports();
                    break;
            }

        }
        private void ReportMonth(int month)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            DateTime lastDayOfMonth = (firstDayOfMonth.AddMonths(1)).AddDays(-1);
            this.ReportFromDate(firstDayOfMonth.Ticks, lastDayOfMonth.Ticks);
        }
        private static bool GetUserMonthInput(out int userInput)
        {            
            userInput = 0;
            bool correctInput = false;
            int attempts = 0;
            while (!correctInput && attempts < 5)
            {
                Console.Clear();
                Console.WriteLine("Input a month between 1-12");
                attempts++;
                bool parseInput = int.TryParse(Console.ReadLine(), out userInput);
                if (parseInput && (userInput <= 12 && userInput >= 1))
                {
                    return true;
                }
                else if (attempts >= 5)
                {
                    Console.WriteLine("Too many incorrect attempts. Returning to main menu");
                    break;
                }
                else
                {
                    Console.WriteLine($"Incorrect input, you input {userInput}");
                    Console.WriteLine("Please enter the Month you wish to view in number form (1-12)");
                    Console.WriteLine("Press any key to try again");
                    Console.ReadLine();
                }
            }
            return false;

        }

        private static void UserInputWait()
        {
            Console.WriteLine("\n Press any key to return to main menu");
            Console.ReadLine();
            Console.Clear();
        }
        private static long GetTodayDate()
        {
            return DateTime.Today.Ticks;
        }


        //this can be used to get the long ticks of whatever date comes 
        // x days before today
        private static long SubtractXDaysFromToday(double xDays)
        {
            DateTime today = new DateTime(GetTodayDate());
            xDays *= -1;
            DateTime XfromToday = today.AddDays(xDays);
            return XfromToday.Ticks;
        }

        private void ReportXDays(double xDays)
        {
            long startDate = SubtractXDaysFromToday(xDays);
            long today = GetTodayDate();
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {today} AND date >= {startDate}";

            using var reader = selectCmd.ExecuteReader();
            long totalHours = 0;
            while (reader.Read())
            {

                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                totalHours += long.Parse(hours);
            }


            Console.WriteLine($"Total Hours between {ParseDate(startDate.ToString())} and {ParseDate(today.ToString())}: {ParseHours(totalHours.ToString())}");
        }

        private void ReportAll()
        {
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time";

            using var reader = selectCmd.ExecuteReader();
            long totalHours = 0;
            while (reader.Read())
            {

                var hours = reader.GetString(1);
                totalHours += long.Parse(hours);
            }


            Console.WriteLine($"All hours total: {ParseHours(totalHours.ToString())} hours");
        }

        private void ReportFromDate(long startDate)
        {
            long today = GetTodayDate();
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {today} AND date >= {startDate}";

            using var reader = selectCmd.ExecuteReader();
            long totalHours = 0;
            while (reader.Read())
            {

                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                totalHours += long.Parse(hours);
            }


            Console.WriteLine($"Total hours from {ParseDate(startDate.ToString())} to Today: {ParseHours(totalHours.ToString())} hours");
        }
        private void ReportFromDate(long startDate, long endDate)
        {
            long today = GetTodayDate();
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM time WHERE date <= {endDate} AND date >= {startDate}";

            using var reader = selectCmd.ExecuteReader();
            long totalHours = 0;
            while (reader.Read())
            {

                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                totalHours += long.Parse(hours);
            }


            Console.WriteLine($"Total hours from {ParseDate(startDate.ToString())} to {ParseDate(endDate.ToString())}: {ParseHours(totalHours.ToString())} hours");
        }
        private string ParseDate(string day)
        {
            DateTime dt = new DateTime(long.Parse(day));
            return dt.ToString("MM-dd-yyyy");
        }

        private string ParseHours(string hours)
        {
            TimeSpan ts = new TimeSpan(long.Parse(hours));
            return ts.ToString("h\\:mm");
        }
        private static long GetUserDate()
        {
            Console.WriteLine("Input a starting date (format: MM-dd-yyyy) ");
            bool finished = false;
            int attempts = 0;
            while (!finished && attempts < 5)
            {
                string userIn = Console.ReadLine();
                Console.Clear();
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                var styles = DateTimeStyles.None;
                finished = DateTime.TryParse(userIn, culture, styles, out DateTime result);
                if (finished)
                {
                    return result.Ticks;
                }
                attempts++;
                if (attempts == 5)
                {
                    Console.WriteLine("Too many incorrect attempts");
                }
                else
                {
                    Console.WriteLine("Incorrect input, try again (format: MM-dd-yyyy)");
                }
            }
            return -1;
        }
        private static bool getUserInt(out double result)
        {
            Console.Clear();
            bool properInput = false;
            while (!properInput)
            {
                Console.WriteLine("How many days ago should the report start from?");
                string userInput = Console.ReadLine();
                properInput = double.TryParse(userInput, out double parseResult);
                if (properInput == true)
                {
                    result = parseResult;
                    return true;
                }
            }
            result = -1;
            return properInput;
        }
    }
}
