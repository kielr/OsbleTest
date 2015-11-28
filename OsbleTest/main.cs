using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsbleTest
{
    class main
    {
        /// <summary>
        /// Main driver for the testing framework w/o NUnit.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            TestingFrameworkMenu menu = new TestingFrameworkMenu();

            //We are being run w/o NUnit, print the menu
            menu.Menu();

            return;
        }
    }
}
