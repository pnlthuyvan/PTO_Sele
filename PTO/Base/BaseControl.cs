using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using PTO.Utilities;
using PTO.Constants;

namespace PTO.Base
{
    public class BaseControl
    {
        private IWebElement wrappedControl;
        protected IWebDriver driver;
        private WebDriverWait wait;
        protected int defaultTimeout = 5;
        protected int overrideTime = 5;

        protected FindType FindType { get; private set; }
        protected string ValueToFind { get; private set; }

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected FindElementHelper FindElementHelper { get; private set; }

        private void SetUpBaseControl(IWebElement control, string findType, string valueToFind, int timeoutSeconds)
        {
            overrideTime = timeoutSeconds;
            if (FindElementHelper == null)
                FindElementHelper = FindElementHelper.Instance(driver);
            switch (findType)
            {
                case "Name":
                    this.FindType = FindType.Name;
                    break;
                case "Id":
                    this.FindType = FindType.Id;
                    break;
                case "CssSelector":
                    this.FindType = FindType.CssSelector;
                    break;
                default:
                    this.FindType = FindType.XPath;
                    break;
            }
            ValueToFind = valueToFind;
            if (wrappedControl != null)
                this.wrappedControl = control;
        }

        protected void CaptureAndLog(string message)
        {
            ExtentReportsHelper.LogInformation(UtilsHelper.CaptureScreen(driver), message);
        }

        #region "Base Control"
        public BaseControl(IWebDriver driver)
        {
            this.driver = driver;
        }

        protected BaseControl(IWebDriver driver, IWebElement control)
        {
            this.driver = driver;
            this.wrappedControl = control;
            this.ValueToFind = GetXpath(control, "");
            SetUpBaseControl(control, "XPath", ValueToFind, 5);
        }

        protected BaseControl(IWebDriver driver, IWebElement wrappedControl, FindType findType, string valueToFind)
        {
            this.driver = driver;
            SetUpBaseControl(wrappedControl, findType: "g", valueToFind, 5);
        }

        protected BaseControl(IWebDriver driver, FindType findType, string valueToFind)
        {
            this.driver = driver;
            SetUpBaseControl(null, findType.ToString("g"), valueToFind, 5);
        }

        protected BaseControl(IWebDriver driver, FindType findType, string valueToFind, int timeoutSeconds)
        {
            this.driver = driver;
            SetUpBaseControl(null, findType: "g", valueToFind, timeoutSeconds);
        }

        public IWebElement RefreshWrappedControl()
        {
            if (this.defaultTimeout != this.overrideTime)
                wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind, this.overrideTime);
            else
                wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind);
            return wrappedControl;
        }
        #endregion

        #region "Functions"

        public string GetXpath(IWebElement childElement, string current)
        {
            if (childElement is null)
                return null;
            string childTag = childElement.TagName;
            if (childTag.Equals("html"))
            {
                return "/html[1]" + current;
            }
            IWebElement parentElement = childElement.FindElement(By.XPath(".."));
            IList<IWebElement> childrenElements = parentElement.FindElements(By.XPath("*")).ToList();
            int count = 0;
            for (int i = 0; i < childrenElements.Count; i++)
            {
                IWebElement childrenElement = childrenElements[i];
                string childrenElementTag = childrenElement.TagName;
                if (childTag.Equals(childrenElementTag))
                {
                    count++;
                }
                if (childElement.Equals(childrenElement))
                {
                    return GetXpath(parentElement, "/" + childTag + "[" + count + "]" + current);
                }
            }
            return null;
        }

        #endregion

        #region "Wait"

        public bool WaitForElementIsInVisible(int timeout, bool captureAndLog = true)
        {
            return UtilsHelper.WaitForElementIsInVisible(driver, FindType, ValueToFind, timeout, captureAndLog);
        }

        #endregion

    }
}
