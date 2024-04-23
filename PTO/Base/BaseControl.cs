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

        protected FindType FindType { get; private set; }
        protected string ValueToFind { get; private set; }

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected FindElementHelper FindElementHelper { get; private set; }

        protected void CaptureAndLog(string message)
        {
            ExtentReportsHelper.LogInfoAndCaptureFullScreen(driver, message);
        }

        #region "Init Base Control"

        /// <summary>
        /// Set up init control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="findType"></param>
        /// <param name="valueToFind"></param>
        private void SetUpBaseControl(IWebElement control, string? findType, string valueToFind)
        {
            FindElementHelper ??= FindElementHelper.Instance(driver);

            this.FindType = findType switch
            {
                "Name" => FindType.Name,
                "Id" => FindType.Id,
                "CssSelector" => FindType.CssSelector,
                _ => FindType.XPath,
            };
            ValueToFind = valueToFind;
            if (wrappedControl != null)
                this.wrappedControl = control;
        }

        public BaseControl(IWebDriver driver)
        {
            this.driver = driver;
        }

        protected BaseControl(IWebDriver driver, IWebElement control)
        {
            this.driver = driver;
            this.wrappedControl = control;
            this.ValueToFind = GetXpath(control, string.Empty);
            SetUpBaseControl(control, "XPath", ValueToFind);
        }

        protected BaseControl(IWebDriver driver, FindType findType, string valueToFind)
        {
            this.driver = driver;
            SetUpBaseControl(null, findType.ToString("g"), valueToFind);
        }

        #endregion

        #region "Wrap control"

        /// <summary>
        /// Refresh control
        /// </summary>
        /// <returns></returns>
        public IWebElement RefreshWrappedControl()
        {
            return wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind);
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

        /// <summary>
        /// Get attribute value of control
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal string GetAttribute(string attribute)
        {
            try
            {
                return GetWrappedControl().GetAttribute(attribute).ToString();
            }
            catch (Exception)
            {
                ExtentReportsHelper.LogWarning($"Can't get attribute '{attribute}'");
                return string.Empty;
            }
        }

        #endregion

        #region "Get attribute"

        public static string GetXpath(IWebElement childElement, string? current)
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

        /// <summary>
        /// Wait until the element is closed
        /// </summary>
        /// <param name="captureAndLog"></param>
        /// <returns></returns>
        public bool WaitForElementIsInVisible(bool captureAndLog = false)
        {
            return UtilsHelper.WaitForElementIsInVisible(driver, FindType, ValueToFind, captureAndLog);
        }

        /// <summary>
        /// Wait until the element is visible
        /// </summary>
        /// <param name="captureAndLog"></param>
        /// <returns></returns>
        public bool WaitForElementIsVisible(bool captureAndLog = false)
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
        public bool IsExisted(bool isCaptured = false)
        {
            UtilsHelper.WaitForElementIsVisible(driver, FindType, ValueToFind);

            // Verifying web element exists on screen...");
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
