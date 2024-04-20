using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Manager;
using OpenQA.Selenium.Remote;
using System.Linq.Expressions;

namespace PTO.Utilities
{
    public class BrowserUtility
    {
        /// <summary>
        /// Init driver with browser from app config file
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public IWebDriver InitDriver(string? browser, string url)
        {

            IWebDriver driver = null;
            try
            {
                switch (browser.ToLower())
                {
                    case "firefox":
                        UtilsHelper.DebugOutput("Configuring test engine using the selected FIREFOX driver...", false);
                        driver = ConfigureFirefoxOptions();
                        break;
                    default:
                        UtilsHelper.DebugOutput("Configuring test engine using the selected FIREFOX driver...", false);
                        driver = ConfigureChromeOptions();
                        break;
                }
                UtilsHelper.DebugOutput("Setting default timeout preferences...", false);

                driver.Manage().Window.Maximize();
                driver.Url = url;

                UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);
                UtilsHelper.DebugOutput($"    ==== Test Engine @ LBM Application ({UtilsHelper.TrimArg(BaseValues.RelativeURL)}) ====", false);
                UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);

                TestEnvironment.active_status = true;
            }
            catch (Exception ex)
            {
                // Handle any initialization errors gracefully
                UtilsHelper.DebugOutput($"Error initializing driver: {ex.Message}", true);
            }
            return driver;
        }


        /*********************************** CHROME HANDLER ************************************************/
        /// <summary>
        /// Set up Chrome driver
        /// </summary>
        /// <returns></returns>
        private static IWebDriver ConfigureChromeOptions()
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            if (BaseValues.Headless)
            {
                UtilsHelper.DebugOutput("Setting web driver to run using the 'headless' flag...", false);
                chromeOptions.AddArgument("--headless=new");
            }

            if (BaseValues.DebugMode)
            {
                UtilsHelper.DebugOutput($"Setting web driver to debug mode on port '{BaseValues.DebugMode}'...", false);
                chromeOptions.AddArgument($"--remote-debugging-port={BaseValues.DebugMode}");
            }

            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--safebrowsing-disable-download-protection");
            chromeOptions.AddUserProfilePreference("safebrowsing.enabled", "true");

            return GetNewChromeDriver(chromeOptions);
        }

        /// <summary>
        /// Init Chrome driver
        /// </summary>
        /// <param name="_driverConfig"></param>
        /// <returns></returns>
        protected static ChromeDriver GetNewChromeDriver(ChromeOptions _driverConfig)
        {
            UtilsHelper.DebugOutput("Generating Selenium Chrome driver...", false);

            ChromeDriverService service;
            service = ChromeDriverService.CreateDefaultService();

            return new ChromeDriver(service, _driverConfig, TimeSpan.FromSeconds(BaseValues.PageLoadTimeOut));
        }


        /*********************************** FIREFOX HANDLER ************************************************/

        /// <summary>
        /// Set up Firefox driver
        /// </summary>
        /// <returns></returns>
        private static IWebDriver ConfigureFirefoxOptions()
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();

            if (BaseValues.Headless)
            {
                UtilsHelper.DebugOutput("Setting web driver to run using the 'headless' flag...", false);
                firefoxOptions.AddArgument("--headless=new");
            }

            if (BaseValues.DebugMode)
            {
                UtilsHelper.DebugOutput($"Setting web driver to debug mode on port '{BaseValues.DebugMode}'...", false);
                firefoxOptions.AddArgument($"--remote-debugging-port={BaseValues.DebugMode}");
            }

            return GetNewFireFoxDriver(firefoxOptions);
        }

        /// <summary>
        /// Init Firefox driver
        /// </summary>
        /// <param name="_driverConfig"></param>
        /// <returns></returns>
        protected static FirefoxDriver GetNewFireFoxDriver(FirefoxOptions _driverConfig)
        {
            UtilsHelper.DebugOutput("Generating Selenium Firefox driver...", false);

            FirefoxDriverService service;
            service = FirefoxDriverService.CreateDefaultService();

            return new FirefoxDriver(service, _driverConfig, TimeSpan.FromSeconds(BaseValues.PageLoadTimeOut));
        }

        /*********************************** DOWNLOAD HANDLER ************************************************/

        /*    private void ConfigureReportLocation(string browser)
            {
                switch (browser.ToLower())
                {
                    case "firefox":
                        break;
                    default:
                        string downloadFolder = BaseValues.WorkingDir + "Download\\";
                        Directory.CreateDirectory(downloadFolder);
                        chromeOptions.AddUserProfilePreference("download.default_directory", downloadFolder);
                        chromeOptions.AddUserProfilePreference("savefile.default_directory", downloadFolder);
                        break;
                }
            }
        */

        /// <summary>
        /// Set up Cookie for driver (run parallel)
        /// </summary>
        /// <param name="driverTest"></param>
        /// <returns></returns>
        public IWebDriver SetUpCookie(IWebDriver driverTest)
        {
            if(BaseValues.LoginCookieList == null)
                return driverTest;

            foreach (var cookie in BaseValues.LoginCookieList)
            {
                driverTest.Manage().Cookies.AddCookie(cookie);
            }
            driverTest.Url = BaseValues.BaseURL;
            return driverTest;
        }
    }
}
