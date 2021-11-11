using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class DatabaseManager
    {
        private SqliteConnection Connection;
        public DatabaseManager(string fileName)
        {
            this.Connection = new SqliteConnection($"Data Source={fileName}");
            this.Connection.Open();
            this.CreateTableIfNonExistent();
        }
        private static long GetTodayDate()
        {
            return DateTime.Today.Ticks;
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

        //TODO: view a single specific date, or the last X days of dates
        public void View(string selection=null)
        {
            var selectCmd = this.Connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM time";
            switch (selection)
            {
                case null:
                    Console.Write("Todays Entry: ");
                    selectCmd.CommandText = $"SELECT * FROM time WHERE date={GetTodayDate()}";
                    break;
                case "all":
                    selectCmd.CommandText = $"SELECT * FROM time";
                    break;
                default:
                    //selectCmd.CommandText = $"SELECT * FROM time WHERE date={day}";
                    break;
            }
            //TODO see if there is an easier/better way to read lines from the DB like this
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var hours = reader.GetString(1);
                var date = reader.GetString(2);
                Console.WriteLine($"Hours: {ParseHours(hours)}, Date: {ParseDate(date)}");
            }
        }
        public void Update(long addHours, long day = -1)
        {
            //TODO make this a switch and model it after the View method
            if (day == -1)
            {
                day = GetTodayDate();
            }
            try
            {
                //TODO tell the user if the date they entered does not exist
                //Maybe give them the chance to create it, if it does not exist?
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

        public void InsertRow(long day, long hoursInput)
        {
            Console.WriteLine(day);
            Console.WriteLine(hoursInput);
            var transaction = this.Connection.BeginTransaction();
            var insertCmd = this.Connection.CreateCommand();
            insertCmd.CommandText = $"INSERT INTO time(hours, date) VALUES({hoursInput},{day})";
            insertCmd.ExecuteNonQuery();

            transaction.Commit();
            Console.WriteLine($"Created row for {day} with {hoursInput} hour(s)");
        }

        private string ParseDate(string day)
        {
            DateTime dt = new DateTime(long.Parse(day));
            return dt.ToString("yyyy-MM-dd");
        }

        private string ParseHours(string hours)
        {
            TimeSpan ts = new TimeSpan(long.Parse(hours));
            return ts.ToString("h\\:mm");
        }
    }
}
