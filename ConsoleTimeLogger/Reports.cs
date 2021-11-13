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
            Console.WriteLine("R for Hours from a start date to today");

            string userInput = Console.ReadLine().ToUpper();
            switch (userInput)
            {
                case "R":
                    bool correct = getUserInt(out double result);
                    if (correct)
                    {
                        ReportXDays(result);
                    }
                    break;
                default:
                    break;
            }

        }
        private static long GetTodayDate()
        {
            return DateTime.Today.Ticks;
        }

        //this should find the date to start from (returned in ticks)
        //if you do not want to start from today
        //example: Get amount of hours between 11-20-2020 and 12-15-2020
        //With this input the end date(12-15-2020) and the subtract of x days to find 11-20-2020
        private static long SubtractDates(long endDate, long subtract)
        {
            DateTime startDate = new DateTime(endDate);
            DateTime subtractDate = new DateTime(subtract);
            TimeSpan diff = startDate.Subtract(subtractDate);
            DateTime finalDate = startDate.Subtract(diff);
            return finalDate.Ticks;

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


            Console.WriteLine(ParseHours(totalHours.ToString()));
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
