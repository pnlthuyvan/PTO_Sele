using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.API.Microsoft;

namespace PTO.TestScripts.Demo.API_Test.Microsoft
{
    [TestFixture]
    [Parallelizable]
    public class ProductAPI_POST : BaseTestScript
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
        public void POST_APIs()
        {
            ExtentReportsHelper.LogInformation($"<font color='lavender'><b>*************** POST *****************</b></font>");
            ProductAPI api = new();

            try
            {
                var reasonPhrase = api.UpdateProductBySection();

                ExtentReportsHelper.LogInformation("Update status: " + reasonPhrase.ToString());
            } catch (Exception e)
            {
                ExtentReportsHelper.LogFail($"<font color='red'>Failed to send API UpdateProductBySection." +
                    $"<br>Exception: {e}</font>");
            }
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
