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
            //Maybe this loop should be in the Main program.cs instead of here?
            bool userWantsExit = false;
            while (!userWantsExit)
            {
                this.DB.View();
                Console.WriteLine("Input your command");
                Console.WriteLine("0 to exit, A to add hours, D to Delete a date, I to insert a date, V to view all");
                string userInputCommand = Console.ReadLine().ToUpper();
                Console.WriteLine("-------------------------------------");
                //TODO create a method to handle the userInput for getting the proper date
                //That function can also verify if a currently used date has been entered
                //which can be a good or bad thing depending on the function
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
            //want to make sure the user inputs a number
            //and give them more than 1 attempt to do it
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
            //a return of -1 lets us know that there was an issue
            return -1;
        }

    }
}