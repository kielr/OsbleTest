using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsbleTest
{
    class main
    {
        static void Main(string[] args)
        {
            UserValidation validateUser = new UserValidation();

            validateUser.DoValidation();
        }
    }
}
