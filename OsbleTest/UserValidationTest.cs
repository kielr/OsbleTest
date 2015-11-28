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
    public class UserValidationTest
    {
        string startDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string password = "cpts422teamosble"; //We have only one password for all test accounts
        public void DoValidation()
        {
            Console.WriteLine("Running ValidateAuthTokenTest()...");
            ValidateAuthTokenTest();

            Console.WriteLine("Running GetCoursesTest()...");
            GetCoursesTest();

            Console.WriteLine("Running GetCourseRoleAsStudent()...");
            GetCourseRoleAsStudent();

            Console.WriteLine("Running SubmitGradeBookAsStudent()...");
            SubmitGradebookAsStudent();

            //Console.WriteLine("Running PostMessageTest()...");
            //PostMessageTest();

            Console.WriteLine("Running DeleteSubmissionTest()...");
            DeleteSubmissionTest();
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
        /// Attempts to get a course role as a student, the role name should be student, etc
        /// </summary>

        [Test]
        public void GetCourseRoleAsStudent()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            string password = "cpts422teamosble"; //We have only one password for all test accounts
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group3@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            Course[] courses = osbleService.GetCourses(authToken);

            CourseRole courseRole = osbleService.GetCourseRole(courses[0].ID, authToken);

            Assert.NotNull(courseRole);


            //No Nunit display
            Console.WriteLine("Returned Course Role ID: " + courseRole.ID);
            Console.WriteLine("Returned Course Role Name: " + courseRole.Name);
            Console.WriteLine("Returned Course Role Permissions : CanUploadFiles = " + courseRole.CanUploadFiles);
            Console.WriteLine("                                   CanGrade       = " + courseRole.CanGrade);
            Console.WriteLine("                                   CanModify      = " + courseRole.CanModify);
            Console.WriteLine("                                   CanSeeAll      = " + courseRole.CanSeeAll);
            Console.WriteLine("                                   CanSubmit      = " + courseRole.CanSubmit);

            StringAssert.AreEqualIgnoringCase("Student", courseRole.Name);
            Assert.IsFalse(courseRole.CanGrade);
            Assert.IsFalse(courseRole.CanUploadFiles);
            Assert.IsFalse(courseRole.CanModify);
            Assert.IsFalse(courseRole.CanSeeAll);
            Assert.IsTrue(courseRole.CanSubmit);

            Console.WriteLine("GetCoureRoleAsStudent() Assert(s) was successful.");
        }

        /// <summary>
        /// Submit a review as a student
        /// </summary>
        [Test]
        public void SubmitGradebookAsStudent()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            int assignmentID = 0;
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group3@gmail.com", password); //Use our OsbleLogin() function from our common test library to retrieve auth tokens
            Course[] courses = osbleService.GetCourses(authToken);
            Assignment[] assignments = osbleService.GetCourseAssignments(courses[0].ID, authToken);

            //Get the file in a zip object
            Console.WriteLine("Zipping file...\n");
            ZipFile file = new ZipFile();
            FileStream stream = File.OpenRead(startDirectory + "\\Test Assignments\\review.xlsx");
            file.AddEntry("review.zip", stream);
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


            int result = osbleService.UploadCourseGradebook(courses[0].ID, zippedFile.ToArray(), authToken);
            Assert.AreEqual(-1, result);
            Console.WriteLine("SubmitGradebookAsStudent() Assert(s) was successful.");
        }

        /// <summary>
        /// Test that we can post a message with PostActivityMessage()
        /// </summary>
        [Test]
        public void PostMessageTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            UploaderWebServiceClient uploader = new UploaderWebServiceClient();
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group4@gmail.com", password);
            Course[] courses = osbleService.GetCourses(authToken);
            bool result = uploader.PostActivityMessage("This", courses[0].ID, authToken);

            Assert.IsTrue(result);
            Console.WriteLine("PostMessageTest() Assert(s) was successful.");
        }

        /// <summary>
        /// Test that we cannot delete a submissions
        /// </summary>
        [Test]
        public void DeleteSubmissionTest()
        {
            OsbleServiceClient osbleService = new OsbleServiceClient();
            UploaderWebServiceClient uploader = new UploaderWebServiceClient();
            string authToken = CommonTestLibrary.OsbleLogin("osble.test.group3@gmail.com", password);
            Course[] courses = osbleService.GetCourses(authToken);

            //student doesn't have delete permissions Student role cannot modify
            bool result = uploader.DeleteFile("hw1.zip", courses[0].ID, authToken);
            Assert.IsFalse(result);
            Console.WriteLine("DeleteSubmissionTest() Assert(s) was successful.");
        }
    }
}
