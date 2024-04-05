using PTO.Base;
using PTO.Manager;
using PTO.Pages.LoginPage;
using PTO.Utilities;

namespace PTO
{
    [SetUpFixture]
    public class TestManager : TestEnvironment
    {
        LoginPage loginPage;

        [OneTimeSetUp]
        public void OnStart()
        {
            UtilsHelper.DebugOutput($"Starting {BaseValues.ProjectName}...", false);
            BaseValues.FirstDriverTest = new BrowserUtility().InitDriver(BaseValues.Browser, BaseValues.BaseURL);

            // Set up default setting for browser
            ApplyDefaultSettings();

            loginPage = new LoginPage(BaseValues.FirstDriverTest);

            // Sign in with user name and pass from config file
            loginPage.SignIn();
            BaseValues.LoginCookieList = GetCookies(BaseValues.FirstDriverTest);
        }

        [OneTimeTearDown]
        public void OnFinish()
        {
            UtilsHelper.DebugOutput("All tests completed...", false);
            UtilsHelper.DebugOutput($"Ending {BaseValues.AppName}...", false);

            loginPage.Logout();
            WriteReportAndDisposeDriver(BaseValues.FirstDriverTest);

            BaseValues.FirstDriverTest?.Dispose();
        }
    }
}
