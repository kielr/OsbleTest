using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsbleTest.Authentication;
using OsbleTest.WebService;
using OsbleTest.AssignmentSubmission;
using OsbleTest.UploaderWebService;
using NUnit.Framework;
using System.IO;
using Ionic.Zip;

namespace OsbleTest
{
    [TestFixture]
    public class UserValidation
    {


        string startDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        public void DoValidation()
        {
            ValidateAuthTokenTest();
            GetCoursesTest();
            SubmitAssignmentCorrectlyTest();
            GetAssignmentTest();
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
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_zip.zip");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //We are looking for A3...
                if (assignment.AssignmentName == "A3 due Dec 25 by 11:55 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            Assert.AreEqual(true, result);
            Console.WriteLine("SubmitAssignmentCorrectlyTest() Assert(s) was successful.");
        }

        /// <summary>
        /// Attempts to download an already submitted assignment.
        /// </summary>
        [Test]
        public void GetAssignmentTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group2@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);
            int assignmentID = 0;

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //We are looking for A3's ID...
                if (assignment.AssignmentName == "A3 due Dec 25 by 11:55 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            ZipFile file = new ZipFile();
            byte[] assignmentData = osbleService.GetAssignmentSubmission(assignmentID, authToken);

            Assert.NotNull(assignmentData);

            Console.WriteLine("GetAssignmentTest() Assert(s) was successful.");
        }


        //Milestone Meeting Notes: 11/2/2015
        //TO DO:The students should be able to submit files to assignments in the courses that correspond to the users
        //      These include the following:
        //              -Submit assignment file ->DONE<-
        //              -Get back submitted file ->DONE<-
        //              -Verify data
        //              -Have some submissions that are expected to work
        //              -And some that aren't
        // For the ones that aren't pay attention to assignment due dates
        // Then the other one is, courses and assignments have ids, submit to this assignment id
        // If student A isn't in course A, then they can't submit to any course A's assignments
        // Submit and be able to pull the submission
        // File size stuff:
        //      -If there is a limit Evan can impose, one test that has junk data that tests that.

        /// <summary>
        /// Attempts to submit an assignment late, it should not submit
        /// </summary>
        [Test]
        public void SubmitAssignmentLateTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group4@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            //Get the file in a zip object
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A1 due Nov 4 by 6 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result);
            Console.WriteLine("SubmitAssignmentLateTest() Assert(s) was successful.");
        }

        /// <summary>
        /// Attempts to submit an assignment with the wrong file type, should not submit
        /// </summary>
        [Test]
        public void SubmitAssignmentWrongFileTypeTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group3@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A2 due Nov 6 by 2 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result);
            Console.WriteLine("SubmitAssignmentWrongFileTypeTest() Assert(s) was successful.");
        }

        /// <summary>
        /// Attempts to submit an assignment to a class student is not in
        /// </summary>
        [Test]
        public void SubmitAssignmentWrongClassTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            // Student that is in class 422
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group4@gmail.com", password);

            // Student that is not in class 422
            string authToken2 = CommonTestLibrary.OsbleLogin("osble.test.group1@gmail.com", password);

            int assignmentID = 0;

            // get the assignments for course 422
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A2 due Nov 6 by 2 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            // the student that was not in 422 is submitting to the assignment ID for an assignment in class 422
            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken2);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result);
            Console.WriteLine("SubmitAssignmentWrongClassTest() Assert(s) was successful.");

        }

        /// <summary>
        /// Attempts to submit a file that is over the teacher imposed 30mb
        /// </summary>
        [Test]
        public void SubmitAssignmentOverSizeLimitTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group4@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            //Get the file in a zip object
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_zip_large.zip");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //We are looking for A3...
                if (assignment.AssignmentName == "A3 due Dec 25 by 11:55 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            Assert.AreEqual(false, result);
            Console.WriteLine("SubmitAssignmentOverSizeLimitTest() Assert(s) was successful.");
        }

    }
}
