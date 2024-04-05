using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace PTO.Utilities
{
    public class FindElementHelper
    {
        private int timeoutSeconds = 5;
        private DefaultWait<IWebDriver> wait;
        private readonly static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IWebDriver driver;
        private static readonly Lazy<FindElementHelper> _lazy = new Lazy<FindElementHelper>(() => new FindElementHelper(driver));

        public FindElementHelper(IWebDriver driverTest)
        {
            driver = driverTest;
        }

        public static FindElementHelper Instance => _lazy.Value;
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


        /*  public void SetTimeoutSeconds(int timeoutSeconds)
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


            public IWebElement FindElementByJavaScript(string js_selector, int find_timeout = 5)
            {
                if (find_timeout != 5) SetWait(find_timeout);
                return FindElement(js_selector);
            }

            public IWebElement FindElement(FindType findElementType, string valueToFind, int timeoutSeconds)
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

            public IWebElement FindElement(FindType findElementType, string valueToFind)
            {
                SetWait(10);
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

            internal IWebElement FindElement(IWebElement element, FindType findElementType, string valueToFind)
            {
                SetWait(5);
                switch (findElementType)
                {
                    case FindType.Name:
                        return FindElement(element, By.Name(valueToFind));
                    case FindType.XPath:
                        return FindElement(element, By.XPath(valueToFind));
                    case FindType.Id:
                        return FindElement(element, By.Id(valueToFind));
                    case FindType.CssSelector:
                        return FindElement(element, By.CssSelector(valueToFind));
                    default:
                        return null;
                }
            }

            internal List<IWebElement> FindElements(IWebElement element, FindType findElementType, string valueToFind)
            {
                SetWait(5);
                switch (findElementType)
                {
                    case FindType.Name:
                        return FindElements(element, By.Name(valueToFind));
                    case FindType.XPath:
                        return FindElements(element, By.XPath(valueToFind));
                    case FindType.Id:
                        return FindElements(element, By.Id(valueToFind));
                    case FindType.CssSelector:
                        return FindElements(element, By.CssSelector(valueToFind));
                    default:
                        return null;
                }
            }

            public List<IWebElement> FindElements(FindType findElementType, string valueToFind, int timeoutSeconds)
            {
                SetWait(timeoutSeconds);
                switch (findElementType)
                {
                    case FindType.Name:
                        return FindElements(By.Name(valueToFind));
                    case FindType.XPath:
                        return FindElements(By.XPath(valueToFind));
                    case FindType.Id:
                        return FindElements(By.Id(valueToFind));
                    case FindType.CssSelector:
                        return FindElements(By.CssSelector(valueToFind));
                    default:
                        return null;
                }
            }

            public List<IWebElement> FindElements(FindType findElementType, string valueToFind)
            {
                SetWait(5);
                switch (findElementType)
                {
                    case FindType.Name:
                        return FindElements(By.Name(valueToFind));
                    case FindType.XPath:
                        return FindElements(By.XPath(valueToFind));
                    case FindType.Id:
                        return FindElements(By.Id(valueToFind));
                    case FindType.CssSelector:
                        return FindElements(By.CssSelector(valueToFind));
                    default:
                        return null;
                }
            }

            private IWebElement FindElement(string javascript_selector)
            {
                try
                {
                    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(WebDriverTimeoutException), typeof(TimeoutException));
                    return wait.Until(x => UtilsHelper.GetElementByJavaScriptQuery(javascript_selector));
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException)
                    {
                        Log.Warning($"The element could not be found by JavaScript - {ex.StackTrace}");
                    }
                    else
                    {
                        Log.Warning($"Unexpected exception occurred while finding element by JavaScript - {ex.StackTrace}");
                    }

                    return null;
                }
            }

            private IWebElement FindElement(IWebElement element, By by)
            {
                if (element is null)
                {
                    Log.Error("The parent element is null so the child element could not be found.");
                    throw new NoSuchElementException("The parent element is null so the child element could not be found.");
                }
                try
                {
                    return wait.Until(x => element.FindElement(by));
                }
                catch (Exception e)
                {
                    Log.Warn($"The element could not be found after {timeoutSeconds} seconds. " + e.Message);
                    return null;
                }
            }


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
                    UtilsHelper.debug_output($"The element could not be found after {timeoutSeconds} seconds. " + e.Message);
                    return null;
                }
            }

            private List<IWebElement> FindElements(IWebElement element, By by)
            {
                try
                {
                    return wait.Until(x => element.FindElements(by)).ToList();
                }
                catch (Exception e)
                {
                    Log.Warning($"The elements could not be found after {timeoutSeconds} seconds. " + e.Message);
                    return null;
                }
            }


            private List<IWebElement> FindElements(By by)
            {
                try
                {
                    return wait.Until(x => x.FindElements(by)).ToList();
                }
                catch (Exception e)
                {
                    Log.Warning($"The elements could not be found after {timeoutSeconds} seconds. " + e.Message);
                    return null;
                }
            }
          */
    }

}
