using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.API;

namespace PTO.TestScripts.API_Test
{
    [TestFixture]
    [Parallelizable]
    public class ProductAPI_POST : BaseTestScript
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
        public void POST_APIs()
        {
            ExtentReportsHelper.LogInformation($"<font color='lavender'><b>*************** POST *****************</b></font>");
            ProductAPI api = new ProductAPI();
            var reasonPhrase = api.UpdateProductBySection();

            ExtentReportsHelper.LogInformation("Update status: " + reasonPhrase.ToString());
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
