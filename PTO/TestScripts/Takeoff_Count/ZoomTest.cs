using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using PTO.Pages.Demo;
using Pipeline.Common.Constants;

namespace PTO.TestScripts.Takeoff_Count
{
    [TestFixture]
    [Parallelizable]
    public class ZoomTest : BaseTestScript
    {
        private DemoPage demo;
        private IWebDriver driverTest;

        private const string JOB_NUMBER = "290224";
        private const string SHEET = "Sheet_1.2";

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.TAKEOFF_COUNT);
        }

        [SetUp]
        public void SetUp()
        {
            driverTest = new BrowserUtility().InitDriver(BaseValues.Browser, BaseValues.ErrorURL);
            UtilsHelper.WaitPageLoad(driverTest);

            foreach (var cookie in BaseValues.LoginCookieList)
            {
                driverTest.Manage().Cookies.AddCookie(cookie);
            }
            driverTest.Url = BaseValues.BaseURL;
            demo = new DemoPage(driverTest);
        }


        [Test, Category($"{Sections.TAKEOFF_COUNT}")]
        public void TestMethod_Zoom_Draw()
        {
            demo.OpenJob(JOB_NUMBER);
            demo.SelectSheet(SHEET);
            demo.ZoomInOut();
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
