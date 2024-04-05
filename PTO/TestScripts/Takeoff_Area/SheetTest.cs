using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using PTO.Pages.LoginPage;
using PTO.Pages.Demo;
using Pipeline.Common.Constants;

namespace PTO.TestScripts.Takeoff_Area
{
    [TestFixture]
    [Parallelizable]
    public class KeyMeasure_Test : BaseTestScript
    {
        private DemoPage demo;
        private IWebDriver driverTest;

        private const string JOB_NUMBER = "290224";
        private const string SHEET = "Sheet_1.2";

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.TAKEOFF_AREA);
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

        [Test, Category("Sheet")]
        public void TestMethod_Select_Sheet()
        {
            demo.OpenJob(JOB_NUMBER);
            demo.SelectSheet(SHEET);
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
