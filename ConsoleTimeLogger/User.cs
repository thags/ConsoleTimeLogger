using System;

namespace ConsoleTimeLogger
{
    class User
    {
        DatabaseManager DB;
        public User(string dbFile)
        {
            this.DB = new DatabaseManager(dbFile);
        }
        public void InputLoop()
        {
            //loop
            bool userWantsExit = false;
            while (!userWantsExit)
            {
                this.DB.View();
                Console.WriteLine("Input your command");
                Console.WriteLine("0 to exit, A to add hours, D to Delete a date, I to insert a date, V to view all");
                string userInputCommand = Console.ReadLine().ToUpper();
                Console.WriteLine("-------------------------------------");
                switch (userInputCommand)
                {
                    case ("0"):
                        userWantsExit = true;
                        break;
                    case "A":
                        int userHourInput = GetUserInt("Add how many hours?: ");
                        this.DB.AddTo(userHourInput);
                        break;
                    case "D":
                        Console.WriteLine("Which date? (yyyyMMdd)");
                        this.DB.Delete(Console.ReadLine());
                        break;
                    case "I":
                        Console.WriteLine("Which date? (yyyyMMdd)");
                        string userDateInput = Console.ReadLine();
                        userHourInput = GetUserInt($"How many hours did you code for {userDateInput}?");
                        this.DB.InsertRow(userDateInput, userHourInput);
                        break;
                    case "V":
                        this.DB.View("all");
                        break;
                    default:
                        Console.WriteLine("Incorrect input, try again.");
                        break;
                }
                Console.WriteLine("-------------------------------------");
            }
        }
        private static int GetUserInt(string message)
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