﻿using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class DatabaseManager
    {
        private SqliteConnection Connection;
        private string todayDate;
        public DatabaseManager(string fileName)
        {
            this.Connection = new SqliteConnection($"Data Source={fileName}");
            this.Connection.Open();
            this.CreateTableIfNonExistent();
            //possibly this should be it's own function that is called each time
            //instead of just at initialization
            this.todayDate = DateTime.Now.Date.ToString("yyyyMMdd");
        }

        public void CreateTableIfNonExistent()
        {
            //we want to make sure that the DB has the proper table
            try
            {
                var createTable = this.Connection.CreateCommand();
                //TODO add a Year, Month, Day column instead of just a date column
                //this will allow easier searching/filtering if added in the future
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
        }

        //TODO: view a single specific date, or the last X days of dates
        public void View(string day=null)
        {
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM time";
            switch (day)
            {
                case null:
                    Console.Write("Todays Entry: ");
                    selectCmd.CommandText = $"SELECT * FROM time WHERE date={this.todayDate}";
                    break;
                case "all":
                    selectCmd.CommandText = $"SELECT * FROM time";
                    break;
                default:
                    selectCmd.CommandText = $"SELECT * FROM time WHERE date={day}";
                    break;
            }
            //TODO see if there is an easier/better way to read lines from the DB like this
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                Console.WriteLine($"Hours: {hours}, Date: {date}");
            }
        }
        public void AddTo(int addHours, string day = null)
        {
            //TODO make this a switch and model it after the View method
            if (day == null)
            {
                day = this.todayDate;
            }
            try
            {
                //TODO tell the user if the date they entered does not exist
                //Maybe give them the chance to create it, if it does not exist?
                var transaction = this.Connection.BeginTransaction();
                var updateCmd = this.Connection.CreateCommand();
                updateCmd.CommandText = $"UPDATE time SET hours=hours+{addHours} WHERE date = {day}";
                updateCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct date?");
            }
        }

        public void Delete(string day)
        {
            try
            {
                //TODO tell the user if the day they entered does not exist
                //do this by using a different checking function
                var transaction = this.Connection.BeginTransaction();
                var deleteCmd = this.Connection.CreateCommand();
                deleteCmd.CommandText = $"DELETE FROM time WHERE date = {day}";
                deleteCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct ID?");
            }
        }

        public void InsertRow(string day, int hours)
        {
            //TODO make this insert a row for todays date if it does not already exist
            //maybe it should be called each time the Add function is called
            if (hours > 0)
            {
                var transaction = this.Connection.BeginTransaction();
                var insertCmd = this.Connection.CreateCommand();
                insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hours}, {day})";
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine($"Created row for {day} with {hours} hour(s)");
            }
        }
    }
}
