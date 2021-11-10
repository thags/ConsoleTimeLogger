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
                Console.WriteLine("0 to exit, A to add hours, S to search, D to delete, U to update");
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
                        DatabaseController.DBSearch(connection);
                        break;
                    case "D":
                        DatabaseController.DeleteItem(connection);
                        break;
                    case "U":
                        DatabaseController.UpdateItem(connection);
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
                    DatabaseController.addHours(connection, addHours);
                    break;
                case ("I"):
                    DatabaseController.InsertInto.InsertHours(connection);
                    break;
                default:
                    Console.WriteLine("Incorrect Input");
                    break;
            }
        }
    }
}
