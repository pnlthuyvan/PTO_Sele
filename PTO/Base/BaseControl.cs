using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using PTO.Utilities;
using PTO.Constants;
using OpenQA.Selenium.Support.Extensions;

namespace PTO.Base
{
    public class BaseControl
    {
        private IWebElement wrappedControl;
        protected IWebDriver driver;
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

        protected BaseControl(IWebDriver driver, FindType findType, string valueToFind)
        {
            this.driver = driver;
            SetUpBaseControl(null, findType.ToString("g"), valueToFind, defaultTimeout);
        }

        #endregion

        #region "Wrap control"

        /// <summary>
        /// Refresh control
        /// </summary>
        /// <returns></returns>
        public IWebElement RefreshWrappedControl()
        {
            if (this.defaultTimeout != this.overrideTime)
                wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind, this.overrideTime);
            else
                wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind);
            return wrappedControl;
        }

        /// <summary>
        /// Get Wrap control
        /// </summary>
        /// <returns></returns>
        public IWebElement GetWrappedControl()
        {
            return RefreshWrappedControl();
        }

        /// <summary>
        /// Get text or value of the element
        /// </summary>
        /// <returns></returns>
        internal string GetTextOrValue()
        {
            try
            {
                return GetWrappedControl().Text == "" ? UtilsHelper.TrimArg(GetWrappedControl().GetAttribute("value")) : UtilsHelper.TrimArg(GetWrappedControl().Text);
            }
            catch (Exception)
            {
                ExtentReportsHelper.LogWarning("Can't get control value.");
                return string.Empty;
            }
        }

        #endregion

        #region "Get attribute"

        public static string GetXpath(IWebElement childElement, string current)
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
            return UtilsHelper.WaitForElementIsInVisible(driver, FindType, ValueToFind, captureAndLog);
        }

        public bool WaitForElementIsVisible(bool captureAndLog = true)
        {
            return UtilsHelper.WaitForElementIsVisible(driver, FindType, ValueToFind, captureAndLog);
        }

        #endregion

        #region "Display/ Exist"

        /// <summary>
        /// Verified the element existed on this screen and log the information
        /// </summary>
        /// <param name="isCaptured"></param>
        /// <returns></returns>
        public bool IsExisted(bool isCaptured = true)
        {
            //Log.Debug("Verifying web element exists on screen...");
            bool isExisted = false;
           UtilsHelper.ActionWithTryCatch(() =>
            {
                if (GetWrappedControl() != null)
                {
                    //Log.Debug($"Successfully detected web element [{FindType:g} : {ValueToFind}]");
                    if (isCaptured)
                        CaptureAndLog($"<font color='green'><b>Successfully</b></font> detected web element <font color='green'><b>[{FindType:g} : {ValueToFind}]</b></font>");//'{GetTextOrValue()}'");
                    isExisted = true;
                }
                else
                {
                    Log.Debug($"Failed to detect web element [{FindType:g} : {ValueToFind}]");
                    if (isCaptured)
                        CaptureAndLog($"<font color='red'><b>Failed</b></font> to detect web element [{FindType:g} : {ValueToFind}]");
                    isExisted = false;
                }
            });
            return isExisted;
        }


        /// <summary>
        /// Verified the element displayed on this screen and log the information
        /// </summary>
        /// <returns>displayed or not</returns>
        public bool IsControlDisplayed(bool isCaptured = true)
        {
            if (!IsExisted(isCaptured))
                return false;

            bool isDisplayed = false;
            UtilsHelper.ActionWithTryCatch(() =>
            {
                IWebElement element = GetWrappedControl();

                bool js_visible = false;
                try { js_visible = driver.ExecuteJavaScript<bool>($"window.getComputedStyle({element}).visibility !== 'hidden';"); } catch { }

                if (element.Displayed || UtilsHelper.IsElementValid(driver, element) || js_visible)
                {
                    isDisplayed = true;
                    if (isCaptured)
                        CaptureAndLog($"Web element [{FindType:g} : {ValueToFind}] is <font color='green'><b>successfully displayed</b></font>.");
                }
                else
                {
                    isDisplayed = false;
                    if (isCaptured)
                        CaptureAndLog($"Web element [{FindType:g} : {ValueToFind}] is <font color='yellow'><b>not displayed</b></font>.");
                }
            });
            return isDisplayed;
        }

        #endregion

    }
}
