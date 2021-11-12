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
    }
}
