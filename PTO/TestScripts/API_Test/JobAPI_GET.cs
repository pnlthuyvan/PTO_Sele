using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.API;
using PTO.Models;

namespace PTO.TestScripts.API_Test
{
    [TestFixture]
    [Parallelizable]
    public class Bearer_GET : BaseTestScript
    {
        private IWebDriver driverTest;

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.PRE_TAKEOFF_MANAGE_JOB);
        }

        [SetUp]
        public void SetUp()
        {
            driverTest = new BrowserUtility().InitDriver(BaseValues.Browser, BaseValues.ErrorURL);
            UtilsHelper.WaitPageLoad(driverTest);

            // Set up cookies using the same instance of BrowserUtility
            driverTest = new BrowserUtility().SetUpCookie(driverTest);
        }

        [Test, Category($"{Sections.PRE_TAKEOFF_MANAGE_JOB}")]
        public void GET_APIs()
        {
            // TakeoffPage.Instance(driverTest).IsTakeoffPageDisplayed();

            JobAPI jobAPI = new JobAPI();
            var reasonPhrase = jobAPI.GetAllJobs();
            string responseContent = reasonPhrase.Result;

            // Convert JSON to Product object
            JobData[] jobDataArray = jobAPI.ConvertJsonToJob(responseContent);

            foreach (var job in jobDataArray)
            {
                ExtentReportsHelper.LogInformation($"<font color='lavender'><b>*************** Job information: {job.Name} *****************</b></font>");

                ExtentReportsHelper.LogInformation("Product Id: " + job.Id);
                ExtentReportsHelper.LogInformation("Name: " + job.Name);
                ExtentReportsHelper.LogInformation("LocationName: " + job.LocationName);
                ExtentReportsHelper.LogInformation("CustomerName: " + job.CustomerName);
                ExtentReportsHelper.LogInformation("EstimatorName: " + job.EstimatorName);
                ExtentReportsHelper.LogInformation("EstimatedValue: " + job.EstimatedValue);
                ExtentReportsHelper.LogInformation("SalesRep: " + job.SalesRep);
                ExtentReportsHelper.LogInformation("Status: " + job.Status);
                ExtentReportsHelper.LogInformation("Notes: " + job.Notes);
                ExtentReportsHelper.LogInformation("Address: " + job.Address);
            }
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
