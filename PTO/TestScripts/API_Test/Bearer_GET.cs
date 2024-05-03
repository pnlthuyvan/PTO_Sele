using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.Pages.TakeoffPage;
using PTO.API;
using PTO.Models;
using RestSharp;

namespace PTO.TestScripts.API_Test
{
    [TestFixture]
    [Parallelizable]
    public class JobAPI_GET : BaseTestScript
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
            Bearer bearer = new Bearer();
            bearer.SetToken();

            // call get request
            RestResponse response = bearer.GetRequest();

            // get content of response
            var content = response.Content;
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
