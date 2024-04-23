using OpenQA.Selenium;
using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using PTO.Pages.TakeoffPage;

namespace PTO.TestScripts.Pre_Takeoff.Manage_Jobs
{
    [TestFixture]
    [Parallelizable]
    public class PTO_1687 : BaseTestScript
    {
        private IWebDriver driverTest;
        private const string JOB_NUMBER = "290224";
        private const string SHEET = "Sheet_1.2";
        private const string MEASUREMENT = "BV_KM_2_Import";

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
        public void Pre_Takeoff_Manage_Job_OpenJob()
        {
            TakeoffPage.Instance(driverTest).IsTakeoffPageDisplayed();
            TakeoffPage.Instance(driverTest).OpenJob(JOB_NUMBER);
            TakeoffPage.Instance(driverTest).SelectSheet(SHEET);
            TakeoffPage.Instance(driverTest).ZoomInOut();
            TakeoffPage.Instance(driverTest).SelectKeyMeasure(MEASUREMENT);
            TakeoffPage.Instance(driverTest).Linear(MEASUREMENT);
        }

        [TearDown]
        public void Dispose()
        {
            driverTest.Dispose();
        }
    }
}
