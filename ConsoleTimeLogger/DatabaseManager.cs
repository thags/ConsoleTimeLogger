using System;
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
            this.todayDate = DateTime.Now.Date.ToString("yyyyMMdd");
        }

        public void CreateTableIfNonExistent()
        {
            //try to create a table
            try
            {
                var createTable = this.Connection.CreateCommand();
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
        public void View()
        {
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM time";
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var key = reader.GetString(0);
                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                Console.WriteLine($"ID: {key}, Hours: {hours}, Date: {date}");
            }
        }
        public void AddTo(int addHours, string day = null)
        {
            if (day == null)
            {
                day = this.todayDate;
            }
            try
            {
                var transaction = this.Connection.BeginTransaction();
                var updateCmd = this.Connection.CreateCommand();
                updateCmd.CommandText = $"UPDATE time SET hours=hours+{addHours} WHERE date = {day}";
                updateCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                Console.WriteLine("Are you sure you inputted the correct ID?");
            }
        }

        public void Delete(string day)
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
                Console.WriteLine("Are you sure you inputted the correct ID?");
            }
        }
    }
}
