using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            //var DB = new DatabaseManager("time.db");
            var User = new User("time.db");
            User.InputLoop();
            //Console.WriteLine(DateTime.Now.Date.ToString("yyyy-MM-dd"));
        }
    }
}
