using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsbleTest.Authentication;

namespace OsbleTest
{
    public class CommonTestLibrary
    {
        public static string OsbleLogin(string username, string password)
        {
            AuthenticationService authClient = new AuthenticationServiceClient();

            return authClient.ValidateUser(username, password);
        }
    }
}
