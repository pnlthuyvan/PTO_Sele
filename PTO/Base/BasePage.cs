using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PTO.Constants;
using PTO.Controls;
using PTO.Utilities;
using WindowsInput;

namespace PTO.Base
{
    public class BasePage(IWebDriver driver)
    {
        protected IWebDriver driverTest = driver;

        protected FindElementHelper ElementFinder => FindElementHelper.Instance(driverTest);
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region "PageLoad"

        protected bool GetJQueryPageInfo()
        {
            bool.TryParse(UtilsHelper.GetJavaScriptResult(driverTest, "return (document.readyState == 'complete');").ToString(), out bool document_state);
            bool.TryParse(UtilsHelper.GetJavaScriptResult(driverTest, "return (typeof jQuery != 'undefined');").ToString(), out bool jquery_exists);

            bool jquery_inactive = true;
            if (jquery_exists)
                jquery_inactive = bool.TryParse(UtilsHelper.GetJavaScriptResult(driverTest, "return (jQuery.active == 0);").ToString(), out _);

            return (document_state && (!jquery_exists || jquery_inactive));
        }

        /// <summary>
        /// Wait for page until the document has loaded and jQuery is completed
        /// </summary>
        /// <param name="timeout_seconds"></param>
        protected void JQueryLoad(int timeout_seconds = 30)
        {
            bool jquery_page_init_state = GetJQueryPageInfo();

            if (timeout_seconds != BaseValues.PageLoadTimeOut) timeout_seconds = BaseValues.PageLoadTimeOut;
            try
            {
                if (jquery_page_init_state)
                {
                    //UtilsHelper.test_output($"JQuery doesn't exists on this page and/or page successfully loaded before wait procedures");
                }
                else
                {
                    var wait = new WebDriverWait(driverTest, TimeSpan.FromSeconds(timeout_seconds));
                    wait.IgnoreExceptionTypes(typeof(WebDriverTimeoutException), typeof(StaleElementReferenceException), typeof(NoSuchElementException), typeof(System.Net.WebException));
                    bool page_state = wait.Until((d) =>
                    {
                        return GetJQueryPageInfo();
                    });

                    if (page_state)
                    {
                        System.Threading.Thread.Sleep(50);

                        wait.Timeout = TimeSpan.FromSeconds(10);
                        bool grid_exists = wait.Until((d) =>
                        {
                            var grid_dom = UtilsHelper.GetJavaScriptResult(driverTest, "return document.querySelector(\".col-md-12 > .card-body-middle\") != undefined;");
                            if (grid_dom != null)
                                return bool.TryParse(grid_dom.ToString(), out _);
                            else
                                return false;
                        });

                        if (grid_exists)
                        {
                            WaitForLoadingAnimation();

                            bool.TryParse(UtilsHelper.GetJavaScriptResult(driverTest, "return (typeof jQuery != 'undefined')").ToString(), out bool jquery_exists);
                            if (jquery_exists)
                            {
                                wait.Timeout = TimeSpan.FromSeconds(5);
                                bool wait_result = wait.Until((d) =>
                                {
                                    var data_result = UtilsHelper.GetJavaScriptResult(driverTest, "return $(\".k-grid[style*='opacity: 1']\").data(\"kendoGrid\").dataSource.total();");
                                    int data_count = (data_result != null) ? int.Parse(data_result.ToString()) : 0;
                                    UtilsHelper.DebugOutput($"Data count in grid at current moment: {data_count}");
                                    return (data_count != 0);
                                });
                                if (wait_result) UtilsHelper.DebugOutput("Successfully waited for kendo grid data...");
                            }

                            System.Threading.Thread.Sleep(500);
                        }
                    }
                    else { UtilsHelper.DebugOutput($"Timed out while loading page, the application is unavailable or there is an issue with the local network connection"); }
                }
            }
            // If it have any unhandle alert displayed, throw new exception and break the test
            catch (UnhandledAlertException e)
            {
                Log.Error($"Failed to wait for JQuery within {timeout_seconds} seconds - Exception: ->\n{e.StackTrace}");
                throw new UnhandledAlertException(string.Format("An unexpected alert is displayed on your page, the test is stopped. The alert is displayed with the message: \"{0}\"", driverTest.SwitchTo().Alert().Text));
            }
            catch (Exception e)
            {
                Log.Error($"Failed to wait for JQuery within {timeout_seconds} seconds - Exception: ->\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Wait for page until the document has loaded.
        /// Does not check for jQuery/AJAX loading, if needed use JQueryLoad instead.
        /// </summary>
        /// <param name="timeout_seconds"></param>
        protected void PageLoad(int timeout_seconds = 30)
        {
            string js_document_state = "return (document.readyState == 'complete');";
            var executeJava = UtilsHelper.GetJavaScriptResult(driverTest, js_document_state);
            if (executeJava == null)
            {
                Log.Error($"Failed to wait for the page to load because of null driver.");
                return;
            }

            bool.TryParse(executeJava.ToString(), out bool ready_state);

            try
            {
                if (!ready_state)
                    if (timeout_seconds != BaseValues.PageLoadTimeOut) timeout_seconds = BaseValues.PageLoadTimeOut;

                //Waiting for the page to load within {timeout_seconds} seconds...

                var wait = new WebDriverWait(driverTest, TimeSpan.FromSeconds(timeout_seconds));
                ready_state = wait.Until((x) => { return bool.TryParse(UtilsHelper.GetJavaScriptResult(driverTest, js_document_state).ToString(), out _); });
            }
            catch (WebDriverTimeoutException e)
            {
                Log.Error($"Failed to wait for the page to load within {timeout_seconds} seconds - Exception: ->\n{e.StackTrace}");
            }
            catch (InvalidOperationException e)
            {
                Log.Error($"The chrome browser closed unexpectedly - Exception: ->\n{e.StackTrace}");
            }
        }

        #endregion

        #region "Loading Icon"

        /// <summary>
        /// Wait until the loadin icon invisible during 60s
        /// </summary>
        /// <returns></returns>
        public bool WaitLoadingIconHide()
        {
            string loadingIcon = "//*[@id='LoadingModal']";
            CommonElement ele = new(driverTest, FindType.XPath, loadingIcon);
            return ele.WaitForElementIsInVisible();
        }

        #endregion

        #region "Common actions"

        /// <summary>
        /// Get title of current page throgh driver
        /// </summary>
        public string Title
        {
            get
            {
                //Log.Debug($"Get the title of the page.");
                return driverTest.Title;
            }
        }

        /// <summary>
        /// Refresh current page through driver
        /// </summary>
        public void RefreshPage()
        {
            driverTest.Navigate().Refresh();
        }

        /// <summary>
        /// Refresh page by click F5 on the keyboard
        /// </summary>
        public static void RefreshAsUser()
        {
            InputSimulator sim = new();
            sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F5);
        }

        /// <summary>
        /// Wait for loading icon displays and closes
        /// </summary>
        /// <returns></returns>
        public bool WaitForLoadingAnimation()
        {
            string loadingIcon = "//*[@id='LoadingModal']";
            CommonElement ele = new(driverTest, FindType.XPath, loadingIcon);

            // Wait until loading icon displays
            ele.WaitForElementIsVisible();

            // Wait until loading icon closes
            return ele.WaitForElementIsInVisible();
        }

        #endregion

        #region "Left Navigation"

        /// <summary>
        /// Navigate to left page
        /// </summary>
        /// <param name="leftMenu"></param>
        /// <param name="selectedSubMenu"></param>
        /// <param name="isCaptured"></param>
        public void NavigateToPage(string leftMenu, string selectedSubMenu, bool isCaptured = true)
        {
            //Log.Debug($"Selecting item in the menu...");
            UtilsHelper.ActionWithTryCatch(() =>
            {
                // Hover left menu on the left navigation
                // Currently, there is only 1 menu
                IWebElement leftMenu = driverTest.FindElement(By.CssSelector("#side-menu"));
                UtilsHelper.HoverMouse(driverTest, leftMenu, isCaptured);

                // Click selected sub menu
                string subMenuSelector = $"//a[@class='active']/span[contains(text(),'{selectedSubMenu}')]";
                UtilsHelper.WaitForElementIsVisible(driverTest, FindType.XPath, subMenuSelector);
                IWebElement itemNeedToClick = driverTest.FindElement(By.XPath(subMenuSelector));

                if (isCaptured)
                    ExtentReportsHelper.LogInformation($"Click item <font color='green'><b><i>{selectedSubMenu}</i></b></font> on left menu.", UtilsHelper.CaptureScreen(driverTest, itemNeedToClick));

                itemNeedToClick.Click();
            });

            JQueryLoad();
        }

        #endregion

        #region "Toast Message"

        public string GetToastMeassage(string messageType)
        {
            string messCss;
            if ("title".Equals(messageType.ToLower()))
            {
                // Toast message title
                messCss = "#toastContainer > div >.sst-message-wrapper > div[class ^= 'sst-toast-header']";
            }
            else
            {
                // Toast message content
                messCss = "#toastContainer > div >.sst-message-wrapper > div[class ^= 'sst-toast-body']";
            }

            string result = string.Empty;

            // Get toast message
            UtilsHelper.WaitForElementIsVisible(driver, FindType.CssSelector, messCss);

            try
            {
                IWebElement toastMessElement = ElementFinder.FindElement(FindType.CssSelector, messCss);
                result = toastMessElement.GetDomProperty("innerText");
            }
            catch (Exception e)
            {
                UtilsHelper.DebugOutput($"Encountered exception while retrieving toast message {messageType} - Exception: ->\n" + e.StackTrace);
            }

            return result;
        }

        #endregion

        #region "Pages"
        /// <summary>
        /// Get the header/title text (title) of the current page
        /// </summary>
        /// <returns></returns>
        public string GetHeaderText()
        {
            IWebElement headerName;
            string headerText;
            try
            {
                headerName = driver.FindElement(By.XPath("//a[contains(@class,'job-title')]"));
                headerText = headerName.Text;
            }
            catch (Exception exception)
            {
                Log.Error($"Could not locate header title element for the current page - Exception: ->\n{exception.StackTrace}");
                headerText = string.Empty;
            }

            return headerText;
        }

        /// <summary>
        /// Get page title from driver
        /// </summary>
        /// <returns></returns>
        public string CurrentTitle
        {
            get
            {
                return driver.Title;
            }
        }

        /// <summary>
        /// Get curent URL of page
        /// </summary>
        public string CurrentURL
        {
            get
            {
                return driver.Url;
            }
        }

        #endregion
    }
}
