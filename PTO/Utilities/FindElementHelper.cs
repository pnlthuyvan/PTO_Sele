using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using PTO.Constants;

namespace PTO.Utilities
{
    public class FindElementHelper
    {
        private int timeoutSeconds = 10;
        private DefaultWait<IWebDriver> wait;
        private readonly static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IWebDriver driver;
        private static readonly Lazy<FindElementHelper> _lazy = new Lazy<FindElementHelper>(() => new FindElementHelper());

        public static FindElementHelper Instance(IWebDriver driverTest)
        {
            driver = driverTest;
            return _lazy.Value;
        }
        
        #region "Find Element"

        private IWebElement FindElement(By by)
        {
            try
            {
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(WebDriverTimeoutException), typeof(TimeoutException), typeof(System.Net.WebException));

                System.Threading.Thread.Sleep(50);
                return wait.Until(x => driver.FindElement(by));
            }
            catch (Exception e)
            {
                UtilsHelper.DebugOutput($"The element could not be found after {timeoutSeconds} seconds. " + e.Message);
                return null;
            }
        }

        public IWebElement FindElement(FindType findElementType, string valueToFind, int timeoutSeconds = 10)
        {
            SetWait(timeoutSeconds);
            switch (findElementType)
            {
                case FindType.Name:
                    return FindElement(By.Name(valueToFind));
                case FindType.XPath:
                    return FindElement(By.XPath(valueToFind));
                case FindType.Id:
                    return FindElement(By.Id(valueToFind));
                case FindType.CssSelector:
                    return FindElement(By.CssSelector(valueToFind));
                default:
                    return null;
            }
        }

        #endregion

        #region "Wait and timeout"

        internal void SetDefaultWait()
        {
            wait = new DefaultWait<IWebDriver>(driver)
            {
                Timeout = TimeSpan.FromSeconds(5),
                PollingInterval = TimeSpan.FromMilliseconds(200)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException));
            wait.Message = "Element to be searched not found";
        }

        public void SetTimeoutSeconds(int timeoutSeconds)
        {
            this.timeoutSeconds = timeoutSeconds;
        }

        private void SetWait(int timeoutSeconds)
        {
            SetTimeoutSeconds(timeoutSeconds);
            if (this.timeoutSeconds != 5)
            {
                wait = new DefaultWait<IWebDriver>(driver)
                {
                    Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                    PollingInterval = TimeSpan.FromMilliseconds(200)
                };
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException));
                wait.Message = "Element to be searched not found";
            }
        }

        #endregion
    }

}
