using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.Models.Microsoft;
using PTO.API.Microsoft;

namespace PTO.TestScripts.Demo.API_Test.Microsoft
{
    [TestFixture]
    [Parallelizable]
    public class JobAPI_GET : BaseTestScript
    {
        private IWebDriver driverTest;

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.API);
        }

        [SetUp]
        public void SetUp()
        {
            driverTest = new BrowserUtility().InitDriver(BaseValues.Browser, BaseValues.ErrorURL);
            UtilsHelper.WaitPageLoad(driverTest);

            // Set up cookies using the same instance of BrowserUtility
            driverTest = new BrowserUtility().SetUpCookie(driverTest);
        }

        [Test, Category($"{Sections.API}")]
        public void GET_APIs()
        {
            JobAPI jobAPI = new();
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
