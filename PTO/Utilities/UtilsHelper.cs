using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using PTO.Base;
using PTO.Constants;
using PTO.Manager;
using System.Diagnostics;

namespace PTO.Utilities
{
    public static class UtilsHelper
    {
        public static log4net.ILog Log;
        public static string log_prefix => $"[{BaseValues.AppName} v{BaseValues.Version}] :";
        private static string cached_style;

        public static void NavigateToLoginPage(IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        #region "Click Hover"

        /// <summary>
        /// Hover mouse on the element
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isCaptured"></param>
        public static void HoverMouse(IWebDriver driver, IWebElement control, bool isCaptured = true)
        {
            ActionWithTryCatch(() =>
            {
                string text = control.Text == "" ? control.GetAttribute("value") : control.Text;
                Actions action = new Actions(driver);
                action.MoveToElement(control).Perform();
                if (isCaptured)
                    ExtentReportsHelper.LogInformation(CaptureScreen(driver, control), $"Hovered on web element '<font color='green'><b><i>{text}</i></b></font>'");
            });
        }
        #endregion

        #region "Wait"

        public static void WaitPageLoad(IWebDriver driver)
        {
            //Wait for the page to load completely using JavaScript
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Wait until getting the element.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="FindType"></param>
        /// <param name="ValueToFind"></param>
        /// <param name="timeout"></param>
        /// <param name="captureAndLog"></param>
        /// <returns></returns>
        public static bool WaitForElementIsVisible(IWebDriver driver, FindType FindType, string ValueToFind, int timeout = 10, bool captureAndLog = true)
        {
            DebugOutput($"Waiting for web element [{FindType:g} : {ValueToFind}] to become visible within {timeout} seconds...");

            IWebElement element;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(WebDriverTimeoutException), typeof(InvalidSelectorException));

            try
            {
                switch (FindType)
                {
                    case FindType.Id:
                        element = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(ValueToFind))));
                        break;
                    case FindType.Name:
                        element = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name(ValueToFind))));
                        break;
                    case FindType.XPath:
                        element = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(ValueToFind))));
                        break;
                    case FindType.CssSelector:
                        element = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(ValueToFind))));
                        break;
                    default:
                        element = null;
                        break;
                }
                if (captureAndLog && element != null && element.Displayed)
                    //CaptureAndLog($"The web element <b>[{FindType:g} : {ValueToFind}]</b> is <font color='green'><b>visible</b></font>.");

                    return element.Displayed;
            }
            catch (Exception e)
            {
                DebugOutput($"Encountered exception while waiting for web element to become visible - Exception: ->\n" + e.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Wait until the element is hidden.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="FindType"></param>
        /// <param name="ValueToFind"></param>
        /// <param name="timeout"></param>
        /// <param name="captureAndLog"></param>
        /// <returns></returns>
        public static bool WaitForElementIsInVisible(IWebDriver driver, FindType FindType, string ValueToFind, int timeout = 20, bool captureAndLog = true)
        {
            DebugOutput($"Waiting for web element [{FindType:g} : {ValueToFind}] to hide within {timeout} seconds...");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(WebDriverTimeoutException));
            try
            {
                bool hide = false;
                switch (FindType)
                {
                    case FindType.Id:
                        hide = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.Id(ValueToFind))));
                        break;
                    case FindType.Name:
                        hide = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.Name(ValueToFind))));
                        break;
                    case FindType.XPath:
                        hide = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath(ValueToFind))));
                        break;
                    case FindType.CssSelector:
                        hide = ReturnActionResultWithTryCatch(() => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(ValueToFind))));
                        break;
                    default:
                        hide = false;
                        break;
                }
                if (captureAndLog)
                    ExtentReportsHelper.LogInformationAndCap(driver, $"Web element <b>[{FindType:g} : {ValueToFind}]</b> is <font color='green'><b>successfully hidden</b></font>.");
                return hide;
            }
            catch (Exception e)
            {
                // UtilsHelper.debug_output($"Encountered exception while waiting for web element to become hidden - Exception: ->\n" + e.StackTrace);
                return false;
            }

        }
        #endregion

        #region "Log"

        /// <summary>
        /// General debugging output using a premade message.
        /// Intended for debugging core components and test setup related methods.
        /// Output is delivered to the debug console and logged to both the ExtentReports and Log files.
        /// NOTE: These do not require DebugMode to be enabled. Use the test_output() methods instead if you need to debug web app related pages or test scripts.
        /// </summary>
        /// <param name="premade_msg"></param>
        public static void DebugOutput(string premade_msg, bool add_to_report = true)
        {
            System.Diagnostics.Debug.WriteLine($"{premade_msg}", log_prefix);
            if (TestEnvironment.active_status && add_to_report)
                ExtentReportsHelper.LogInformationAndCap(null, premade_msg);
            else Debug.WriteLine($"{premade_msg}", log_prefix);

            if (!TestEnvironment.active_status && Log != null) Log.Debug($"{log_prefix}: {premade_msg}");
        }

        /// <summary>
        /// Init log
        /// </summary>
        internal static void InitLog()
        {
            if (Log == null)
                Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        #endregion

        #region "Path"

        /// <summary>
        /// Handle debug out put message with input is path
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string TrimArg(string message)
        {
            if (message == null) return string.Empty;
            return (message.Length > 28) ? message[..24].TrimEnd() + ".." : message;
        }

        #endregion

        #region "Capture screen"

        /// <summary>
        /// Capture screen and highlight the control, if does not have the control, the system will capture current screen
        /// </summary>
        /// <param name="iControl"></param>
        /// <returns></returns>
        public static string CaptureScreen(IWebDriver driver, IWebElement control = null)
        {
            HighLightElement(driver, control);

            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();

            bool exists = Directory.Exists($"{BaseValues.WorkingDir}\\ScreenShots");
            if (!exists)
                Directory.CreateDirectory($"{BaseValues.WorkingDir}\\ScreenShots");

            string finalpth = $"./ScreenShots/{DateTime.Now:hh-mm-ssff}.png";

            //string localpath = new Uri(BaseValues.PathReportFile + finalpth).LocalPath;
            screenshot.SaveAsFile(BaseValues.WorkingDir + finalpth);

            RemoveHighLightElement(driver, control);
            return finalpth;
        }

        #endregion

        #region "2light"

        /// <summary>
        /// Highlight function that supported for capturing function
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="control"></param>
        internal static void HighLightElement(IWebDriver driver, IWebElement control = null)
        {
            if (control != null && driver != null)
            {
                try
                {
                    cached_style = control.GetAttribute("style");
                    Log.Debug($"Highlighting web element...");
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    js.ExecuteScript("arguments[0].scrollIntoView({block: 'end'});", control);
                    js.ExecuteScript("arguments[0].focus();", control);

                    js.ExecuteScript($"arguments[0].style='border: 4px solid red; {cached_style}'", control);
                }
                catch (Exception e)
                {
                    Log.Warn($"Failed to highlight web element, it is out of date on the DOM - Exception: ->\n" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Remove highlight function that supported for capturing function
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="control"></param>
        internal static void RemoveHighLightElement(IWebDriver driver, IWebElement control)
        {
            if (control != null && driver != null)
            {
                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript($"arguments[0].style='{cached_style}'", control);
                }
                catch (Exception e)
                {
                    //Log.Warn($"Failed to remove highlight from web element, it is out of date on the DOM - Exception: ->\n" + e.StackTrace);
                }
                finally
                {
                    cached_style = string.Empty;
                }
            }
        }
        #endregion

        #region "JAVA SCRIPT"

        /// <summary>
        /// Get result of a JavaScript query in the form of Object
        /// </summary>
        public static object? GetJavaScriptResult(IWebDriver driver, string script)
        {
            //UtilsHelper.debug_output($"Executing expression by JavaScript...");

            try
            {
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                return executor.ExecuteScript(script);
            }
            catch (Exception ex)
            {
                UtilsHelper.DebugOutput($"Encountered exception while executing JavaScript '{script}' - Exception: ->\n{ex.StackTrace}");
                return null;
            }
        }

        #endregion

        #region "Try catch"

        /// <summary>
        /// Handle Try catch methods
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="StaleElementReferenceException"></exception>
        /// <exception cref="ElementNotInteractableException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        public static TResult ReturnActionResultWithTryCatch<TResult>(Func<TResult> action)
        {
            try
            {
                return action();
            }
            catch (StaleElementReferenceException e)
            {
                //Log.Error($"The element [{FindType:g}|{ValueToFind}] is out of date - " + e.Message);
                throw new StaleElementReferenceException("The element is out of date");
            }
            catch (ElementNotInteractableException e)
            {
                //Log.Error($"The element [{FindType:g}|{ValueToFind}] is not interactable - " + e.Message);
                throw new ElementNotInteractableException("The element is not interactable (hidden or disabled...)");
            }
            catch (NoSuchElementException e)
            {
                //UtilsHelper.debug_output($"The element [{FindType:g}|{ValueToFind}] does not exist on DOM - " + e.Message);
                throw new NoSuchElementException("The element could not be found");
            }
        }


        public static void ActionWithTryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (StaleElementReferenceException e)
            {
                Log.Error($"The web element is out of date - Exception: ->\n" + e.StackTrace);
                // throw new StaleElementReferenceException("The element is out of date");
            }
            catch (ElementNotInteractableException e)
            {
                Log.Error($"Uninteractable web element - Exception: ->\n" + e.StackTrace);
                // throw new ElementNotInteractableException("Uninteractable web element (hidden, disabled, or unusable...)");
            }
            catch (NoSuchElementException e)
            {
                Log.Warn($"The web element does not exist on the DOM - Exception: ->\n" + e.StackTrace);
                // throw new NoSuchElementException("The element could not be found");
            }
        }
        #endregion

    }
}
