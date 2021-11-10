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
                    DatabaseController.Edit.AddHours(connection, addHours);
                    break;
                case ("I"):
                    Console.WriteLine("What day would you like to add to(Format of: yyyyMMdd)");
                    string day = Console.ReadLine();
                    Console.WriteLine("Insert how many hours you want to add to today");
                    userAddHoursInput = Console.ReadLine();
                    addHours = Convert.ToInt32(userAddHoursInput);
                    DatabaseController.Edit.AddHours(connection, addHours, day);
                    break;
                default:
                    Console.WriteLine("Incorrect Input");
                    break;
            }
        }

        //I want to handle all user input here
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
            Console.WriteLine("Input the Hours it should be updated to");
            string hoursUpdateInput = Console.ReadLine();
            int hoursUpdate = Convert.ToInt16(hoursUpdateInput);
            DatabaseController.UpdateItem(connection, hoursUpdate, updateDate);
            Console.WriteLine("Updated Line:");
            DatabaseController.View.DBSearch(connection, updateDate);
        }
    }
}
