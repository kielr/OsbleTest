using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Test change

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
            UserValidation validateUser = new UserValidation();

            validateUser.DoValidation();
        }
    }
}
