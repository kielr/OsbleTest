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

namespace OsbleTest
{
    [TestFixture]
    public class UserValidation
    {

        public void DoValidation()
        {
            ValidateAuthTokenTest();
            GetCoursesTest();
        }
        
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
            
        }


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

            //Milestone Meeting Notes: 11/2/2015
            //TO DO:The students should be able to submit files to assignments in the courses that correspond to the users
            //      These include the following:
            //              -Submit assignment file
            //              -Get back submitted file
            //              -Verify data
            //              -Have some submissions that are expected to work
            //              -And some that aren't
            // For the ones that aren't pay attention to assignment due dates
            // Then the other one is, courses and assignments have ids, submit to this assignment id
            // If student A isn't in course A, then they can't submit to any course A's assignments
            // Submit and be able to pull the submission
            // File size stuff:
            //      -If there is a limit Evan can impose, one test that has junk data that tests that.
        }
        [Test]
        public void SubmitAssignmentCorrectly()
        {
            AuthenticationServiceClient authenticationService = new AuthenticationServiceClient();
            AssignmentSubmissionServiceClient assignmentService = new AssignmentSubmissionServiceClient();
            OsbleServiceClient osbleService = new OsbleServiceClient();

            string password = "cpts422teamosble";
            string authToken = authenticationService.ValidateUser("osble.test.group2@gmail.com", password);
            byte[] zippedFile = File.ReadAllBytes("E:\\Documents\\GitHub\\OsbleTest\\OsbleTest\\HW2 instructions.zip");
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Console.WriteLine("Course: " + courses[0].Name);
            Assignment[] assignments = courses[0].Assignments;
            Console.WriteLine("assignments length " + assignments.Length);
            Console.WriteLine("Assignment 1 " + assignments[0].AssignmentName);
            foreach (Assignment assignment in assignments)
            {
                Console.WriteLine("Assignment Name "+ assignment.AssignmentName);
                Console.WriteLine("Assignment ID " + assignment.ID);
                if (assignment.AssignmentName == "A3")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile, authToken);
            Assert.AreEqual(true, result);






        }

    }
}
