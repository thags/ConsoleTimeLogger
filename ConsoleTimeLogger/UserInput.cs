using System;
using Microsoft.Data.Sqlite;

namespace ConsoleTimeLogger
{
    class UserInput
    {
        public static void InputLoop(SqliteConnection connection)
        {
            //loop
            int userInput = 1;
            while (userInput != 0)
            {
                Console.WriteLine("Input your command");
                Console.WriteLine("0 to exit, A to add hours, S to search, D to delete, U to update, V to view all");
                string userInputCommand = Console.ReadLine().ToUpper();
                Console.WriteLine("-------------------------------------");
                switch (userInputCommand)
                {
                    case ("0"):
                        userInput = 0;
                        break;
                    case "A":
                        InsertUserInput(connection);
                        break;
                    case "S":
                        ViewDB(connection, true);
                        break;
                    case "D":
                        DatabaseController.DeleteItem(connection);
                        break;
                    case "U":
                        DatabaseController.UpdateItem(connection);
                        break;
                    case "V":
                        DatabaseController.View.ViewAll(connection);
                        break;
                    default:
                        Console.WriteLine("Incorrect input, try again.");
                        break;
                }
                Console.WriteLine("-------------------------------------");
            }
        }

        public static void InsertUserInput(SqliteConnection connection)
        {
            Console.WriteLine("T to add hours for today, or I to add hours to a different day");
            string userInput = Console.ReadLine().ToUpper();
            switch (userInput)
            {
                case ("T"):
                    Console.WriteLine("Insert how many hours you want to add to today");
                    string userAddHoursInput = Console.ReadLine();
                    int addHours = Convert.ToInt32(userAddHoursInput);
                    DatabaseController.Edit.addHours(connection, addHours);
                    break;
                case ("I"):
                    DatabaseController.Edit.InsertHours(connection);
                    break;
                default:
                    Console.WriteLine("Incorrect Input");
                    break;
            }
        }

        public static void ViewDB(SqliteConnection connection, bool requireInput=false)
        {
            DatabaseController.View.DBSearch(connection);
            string day = null;
            if (requireInput)
            {
                Console.WriteLine("Which day would you like to search for? (Enter in yyyyMMdd format)");
                day = Console.ReadLine();
            }
            DatabaseController.View.DBSearch(connection, day);
        }
    }
}
