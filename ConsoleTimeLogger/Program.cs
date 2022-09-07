using System;
using Microsoft.Data.Sqlite;

// Test Comment Here
namespace ConsoleTimeLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var User = new User("time.db");
            User.InputLoop();
        }
    }
}
