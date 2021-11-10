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
                        AddOperationUserInput(connection);
                        break;
                    case "S":
                        ViewDB(connection, true);
                        break;
                    case "D":
                        UserDelete(connection);
                        break;
                    case "U":
                        UserUpdate(connection);
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

        public static void AddOperationUserInput(SqliteConnection connection)
        {
            Console.WriteLine("T to add hours for today, or I to add hours to a different day");
            string userInput = Console.ReadLine().ToUpper();
            switch (userInput)
            {
                case ("T"):
                    int addToday = GetUserInt("Insert how many hours you want to add to today: ");
                    DatabaseController.Edit.AddHours(connection, addToday);
                    DatabaseController.View.DBSearch(connection);
                    break;
                case ("I"):
                    Console.WriteLine("What day would you like to add to(Format of: yyyyMMdd)");
                    string day = Console.ReadLine();
                    int addToDay = GetUserInt("Insert how many hours you want to add to that day: ");
                    DatabaseController.Edit.AddHours(connection, addToDay, day);
                    DatabaseController.View.DBSearch(connection, day);
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

        public static void UserDelete(SqliteConnection connection)
        {
            DatabaseController.View.ViewAll(connection);
            Console.WriteLine("Input the ID of the item you wish to delete");
            string deleteID = Console.ReadLine();
            DatabaseController.Delete.DeleteItem(connection, deleteID);
        }

        public static void UserUpdate(SqliteConnection connection)
        {
            DatabaseController.View.ViewAll(connection);
            Console.WriteLine("Which day would you like to update? (Enter in yyyyMMdd format)");
            string updateDate = Console.ReadLine();
            int userResponse = GetUserInt("Input the hours it should be updated to: ");
            DatabaseController.UpdateItem(connection, userResponse, updateDate);
            Console.WriteLine("Updated Line:");
            DatabaseController.View.DBSearch(connection, updateDate);


        }
        public static int GetUserInt(string message)
        {
            bool isNumber = false;
            int attemptCounter = 0;
            while (!isNumber)
            {
                Console.WriteLine(message);
                string intInput = Console.ReadLine();
                isNumber = int.TryParse(intInput, out int intOutput);
                if (isNumber)
                {
                    return intOutput;
                }
                else if (attemptCounter > 5)
                {
                    Console.WriteLine("Too many incorrect inputs.");
                    break;
                }
                else
                {
                    attemptCounter++;
                    Console.WriteLine("Make sure to input a number");
                    Console.WriteLine("Try Again");
                }
            }
            return -1;
        }
    }
}
