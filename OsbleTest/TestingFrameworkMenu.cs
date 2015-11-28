using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsbleTest
{
    class TestingFrameworkMenu
    {
        public void Menu()
        {
            string userInput;
            int convertedUserInput;
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("You are running the tests w/o NUnit, \npress a number and enter to run a test fixture:\n\n");
                Console.WriteLine("0 = Quit program");
                Console.WriteLine("1 = UserValidation Test Fixture");
                Console.WriteLine("2 = AssignmentTest Test Fixture\n\n");
                Console.Write("Your choice: ");
                userInput = Console.ReadLine();
                convertedUserInput = int.Parse(userInput);

                switch (convertedUserInput)
                {
                    case 0:
                        running = false;
                        break;
                    case 1:
                        UserValidationTest userValidation = new UserValidationTest();
                        userValidation.DoValidation();
                        Console.Write("\n\nPlease press enter to continue...");
                        Console.ReadLine();
                        break;
                    case 2:
                        AssignmentTest assignmentTest = new AssignmentTest();
                        assignmentTest.DoAssignmentTests();
                        Console.Write("\n\nPlease press enter to continue...");
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("ERROR: Invalid Input");
                        break;
                }
            }



            return;
        }
    }
}
