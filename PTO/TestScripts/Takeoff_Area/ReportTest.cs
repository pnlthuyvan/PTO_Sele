using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using PTO.Pages.Demo;
using Pipeline.Common.Constants;

namespace PTO.TestScripts.Takeoff_Area
{
    [TestFixture]
    //[Parallelizable]
    public class ReportTest : BaseTestScript
    {
        private DemoPage demo;

        // private IWebDriver firstDriverTest;
        public IWebDriver driverTest;

        private const string JOB_NUMBER = "290224";

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.TAKEOFF_AREA);
        }

        [SetUp]
        public void SetUp()
        {
            driverTest = new BrowserUtility().InitDriver(BaseValues.Browser, BaseValues.ErrorURL);
            UtilsHelper.WaitPageLoad(driverTest);

            // Set up cookies using the same instance of BrowserUtility
            driverTest = new BrowserUtility().SetUpCookie(driverTest);
            demo = new DemoPage(driverTest);
        }

        [Test, Category($"{Sections.TAKEOFF_AREA}")]
        public void TestMethod_Report()
        {
            demo.OpenJob(JOB_NUMBER);
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
