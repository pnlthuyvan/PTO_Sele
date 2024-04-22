using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using PTO.Pages.Demo;
using Pipeline.Common.Constants;

namespace PTO.TestScripts.Demo
{
    [TestFixture]
    [Parallelizable]
    public class LinearTest : BaseTestScript
    {
        private DemoPage demo;
        private IWebDriver driverTest;

        private const string JOB_NUMBER = "290224";
        private const string SHEET = "Sheet_1.2";
        private const string MEASUREMENT = "BV_KM_2_Import";

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.TAKEOFF_LINEAR);
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


        [Test, Category($"{Sections.TAKEOFF_LINEAR}")]
        public void TestMethod_Draw_Linear()
        {
            demo.OpenJob(JOB_NUMBER);
            demo.SelectSheet(SHEET);

            demo.ZoomInOut();
            demo.SelectKeyMeasure(MEASUREMENT);
            demo.Linear();
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
