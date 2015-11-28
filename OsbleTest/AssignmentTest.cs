using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsbleTest.Authentication;
using OsbleTest.WebService;
using OsbleTest.AssignmentSubmission;
using OsbleTest.UploaderWebService;
using System.IO;
using Ionic.Zip;
using NUnit.Framework;

namespace OsbleTest
{
    [TestFixture]
    class AssignmentTest
    {
        string startDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string password = "cpts422teamosble"; //We have only one password for all test accounts

        public void DoAssignmentTests()
        {
            Console.WriteLine("Running SubmitAssignmentCorrectlyTest()...");
            SubmitAssignmentCorrectlyTest();
            Console.WriteLine("Running SubmitAssignmentLateTest()...");
            SubmitAssignmentLateTest();
            Console.WriteLine("Running SubmitAssignmentOverSizeLimitTest()...");
            SubmitAssignmentOverSizeLimitTest();
            Console.WriteLine("Running SubmitAssignmentWrongClassTest()...");
            SubmitAssignmentWrongClassTest();
            Console.WriteLine("SubmitAssignmentWrongFileTypeTest()");
            SubmitAssignmentWrongFileTypeTest();
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
            Console.WriteLine("Zipping file...\n");
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);
            Console.WriteLine("File zipped.\n");

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A1 due Nov 4 by 6 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            Console.WriteLine("Attempting to submit assignment now...\n");

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result, "OSBLE accepted assignment late when it should not.");
            Console.WriteLine("SubmitAssignmentLateTest() Assert(s) successful. Assignment was not submitted.");
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

            // Zip file
            Console.WriteLine("Zipping file...\n");
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);
            Console.WriteLine("File zipped.\n");

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A2 due Nov 6 by 2 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            Console.WriteLine("Attempting to submit assignment now...\n");

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result, "OSBLE accepted assignment with wrong file type when it should not.");
            Console.WriteLine("SubmitAssignmentWrongFileTypeTest() Assert(s) was successful. Assignment was not submitted.");
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

            Console.WriteLine("Zipping file...\n");
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_txt.txt");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);
            Console.WriteLine("File zipped.\n");

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //Find a passed due assignment
                if (assignment.AssignmentName == "A2 due Nov 6 by 2 PM")
                {
                    assignmentID = assignment.ID;
                }
            }

            Console.WriteLine("Attempting to submit assignment now...\n");

            // the student that was not in 422 is submitting to the assignment ID for an assignment in class 422
            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken2);
            // SubmitAssignment should return false
            Assert.AreEqual(false, result, "OSBLE accepted assignment to wrong class when it should not.");
            Console.WriteLine("SubmitAssignmentWrongClassTest() Assert(s) was successful. Assignment was not submitted.");

        }

        /// <summary>
        /// Attempts to submit a file that is over the teacher imposed 30mb
        /// </summary>
        [Test]
        public void SubmitAssignmentOverSizeLimitTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group4@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            int assignmentID = 0;
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            //Get the file in a zip object
            Console.WriteLine("Zipping file...\n");
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\deliverable_zip_large.zip");
            file.AddEntry("hw1.zip", stream);
            MemoryStream zippedFile = new MemoryStream();
            file.Save(zippedFile);
            Console.WriteLine("File zipped.\n");

            //For each assignment in assignments...
            foreach (Assignment assignment in assignments)
            {
                //We are looking for A3...
                if (assignment.AssignmentName == "A3 due Dec 25 by 11:55 PM")
                {
                    assignmentID = assignment.ID;
                    Console.WriteLine("Successfully found assignment.\n");
                }
            }

            Console.WriteLine("Attempting to submit assignment now...\n");

            bool result = osbleService.SubmitAssignment(assignmentID, zippedFile.ToArray(), authToken);
            Assert.AreEqual(false, result, "OSBLE accepted oversize assignment when it should not.");
            Console.WriteLine("SubmitAssignmentOverSizeLimitTest() Assert(s) successful. Assignment was not submitted.");
        }

        /// <summary>
        /// Submits an assignment, then compares it to the original file
        /// </summary>
        [Test]
        public void VerifyAssignmentContentsTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group3@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
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


            // Retrieve the assignment

            byte[] expected = zippedFile.ToArray();
            byte[] results = osbleService.GetAssignmentSubmission(assignmentID, authToken);

            Console.WriteLine("Expected Length: " + expected.Length + "\nActual Length: " + results.Length);

            Assert.AreEqual(expected.Length, results.Length);
            for (int i = 70; i < expected.Length - 100; i++)
            {
                Assert.AreEqual(expected[i], results[i]);
            }
            Console.WriteLine("VerifyAssignmentContentsTest() Assert(s) was successful.");

        }
    }
}
