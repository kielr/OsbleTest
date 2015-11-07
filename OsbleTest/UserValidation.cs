using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsbleTest.Authentication;
using OsbleTest.WebService;
using OsbleTest.AssignmentSubmission;
using NUnit.Framework;
using System.IO;
using Ionic.Zip;

namespace OsbleTest
{
    [TestFixture]
    public class UserValidation
    {

        public void DoValidation()
        {
            ValidateAuthTokenTest();
            GetCoursesTest();
            SubmitAssignmentCorrectlyTest();
        }
        
        /// <summary>
        /// Attempts to retrieve an AuthToken and makes sure it isn't NULL
        /// </summary>
        [Test]
        public void ValidateAuthTokenTest()
        {
            // Variable declarations
            List<string> uNames = new List<string>();
            List<string> receivedAuthTokens = new List<string>();
            string password = "cpts422teamosble";

            for (int i = 1; i < 6; i++)
            {
                uNames.Add("osble.test.group" + i + "@gmail.com");
            }


            // Run Tests
            for (int i = 1; i < 6; i++)
            {
                //Get auth tokens using OsbleLogin() from CommonTestLibrary which returns a string containing an auth token
                receivedAuthTokens.Add(CommonTestLibrary.OsbleLogin(uNames[i - 1], password));
            }


            // Validate data
            for (int i = 1; i < 6; i++)
            {
                //If the auth token is null, stop the function
                Assert.IsNotNull(receivedAuthTokens[i - 1]);
            }

            Console.WriteLine("ValidateAuthTokenTest() Assert(s) was successful.");

        }

        /// <summary>
        /// Attempts to get a list of courses, and then makes sure they are correct
        /// </summary>
        [Test]
        public void GetCoursesTest()
        {
            // Variable declarations
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string[] expectedClasses = { "422 SE Principles II", "Fake 322" };
            int[] expectedCourseCount = { 0, 1, 1, 2, 2 };
            List<Course[]> receivedClasses = new List<Course[]>();
            List<string> uNames = new List<string>();
            string receivedAuthToken;
            string password = "cpts422teamosble";

            //Create a table of user names
            for (int i = 1; i < 6; i++)
            {
                uNames.Add("osble.test.group" + i + "@gmail.com");
            }

            //Receive an AuthToken, then immediately call GetCourses with that AuthToken
            //Store the Class[] in a List
            for (int i = 0; i < 5; i++)
            {
                receivedAuthToken = CommonTestLibrary.OsbleLogin(uNames[i], password);
                receivedClasses.Add(osbleService.GetCourses(receivedAuthToken));
            }

            //Loop through the classes
            for (int i = 0; i < 5; i++)
            {
                //Assert that the number of classes are equal
                Assert.AreEqual(expectedCourseCount[i], receivedClasses[i].Length);
                for (int j = 0; j < expectedCourseCount[i]; j++)
                {
                    //Assert the strings are equal
                    StringAssert.AreEqualIgnoringCase(expectedClasses[j], receivedClasses[i][j].Name);
                }
            }

            Console.WriteLine("GetCoursesTest() Assert(s) was successful.");
        }

        /// <summary>
        /// Attempts to submit an assignment to an assignment that is not late...
        /// </summary>
        [Test]
        public void SubmitAssignmentCorrectlyTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group2@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            //Get the file in a zip object
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead("C:\\Users\\regus_000\\Source\\Repos\\OsbleTest\\OsbleTest\\HW2 instructions.zip");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //We are looking for a not late assignment...
                if (assignment.AssignmentName == "A3 due Dec 25 by 11:55 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            Assert.AreEqual(true, result);
            Console.WriteLine("SubmitAssignmentCorrectlyTest() Assert(s) was successful.");
        }

    }
}
