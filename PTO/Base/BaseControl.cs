using Microsoft.VisualBasic.FileIO;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using PTO.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using PTO.Constants;

namespace PTO.Base
{
    public class BaseControl
    {
        /*private static readonly ILogger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        private IWebElement wrappedControl;
        private WebDriverWait wait;

        protected FindElementHelper FindElementHelper { get; private set; }
        protected int defaultTimeout = 5;
        protected int overrideTime = 5;
        protected FindType FindType { get; private set; }
        protected string ValueToFind { get; private set; }

        /// <summary>
        /// Get text or value of the element
        /// </summary>
        /// <returns></returns>
        internal string GetTextOrValue()
        {
            try
            {
                return GetWrappedControl().Text == "" ? UtilsHelper.trim_arg(GetWrappedControl().GetAttribute("value")) : UtilsHelper.trim_arg(GetWrappedControl().Text);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void SetUpBaseControl(IWebElement wrappedControl, string findType, string valueToFind, int timeoutSeconds)
        {
            overrideTime = timeoutSeconds;
            if (driver == null)
                driver = BaseValues.DriverSession;
            if (FindElementHelper == null)
                FindElementHelper = FindElementHelper.Instance();
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
                this.wrappedControl = wrappedControl;
        }

        protected void CaptureAndLog(string message)
        {
            ExtentReportsHelper.LogInformation(UtilsHelper.CaptureScreen(this), message);
        }

        protected BaseControl(IWebElement control)
        {
            wrappedControl = control;
            ValueToFind = GetXpath(control, "");
            SetUpBaseControl(control, "XPath", ValueToFind, 5);
        }

        protected BaseControl(IWebElement wrappedControl, FindType findType, string valueToFind)
        {
            SetUpBaseControl(wrappedControl, findType: "g", valueToFind, 5);
        }

        protected BaseControl(FindType findType, string valueToFind)
        {
            SetUpBaseControl(null, findType.ToString("g"), valueToFind, 5);
        }

        protected BaseControl(FindType findType, string valueToFind, int timeoutSeconds)
        {
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

        public IWebElement GetWrappedControl()
        {
            return RefreshWrappedControl();
        }

        public string GetAttribute(string attributeName, bool isCaptured = true)
        {
            //Log.Debug($"Retrieving value property of control [{FindType:g} : {ValueToFind}]");
            string text = string.Empty;
            ActionWithTryCatch(() =>
            {
                text = GetWrappedControl().GetAttribute(attributeName);
                if (isCaptured)
                    CaptureAndLog($"Successfully retrieved web element attribute '<font color='green'><b>{text}</b></font>'");
            });
            return text;
        }

        public string GetValue(string member)
        {
            string attribute_value = string.Empty;

            try { attribute_value = this.GetWrappedControl().GetProperty(member); }
            catch { }

            return attribute_value;
        }

        /// <summary>
        /// Verified the element existed on this screen and log the information
        /// </summary>
        /// <param name="isCaptured"></param>
        /// <returns></returns>
        public bool IsExisted(bool isCaptured = true)
        {
            //Log.Debug("Verifying web element exists on screen...");
            bool isExisted = false;
            ActionWithTryCatch(() =>
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
        /// Verified the element Is Enabled on this screen and log the information
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled(bool isCaptured = true)
        {
            //Log.Debug($"Retrieving web element's 'Enabled' property [{FindType:g} : {ValueToFind}]");
            bool isEnabled = false;
            ActionWithTryCatch(() =>
            {
                isEnabled = GetWrappedControl().Enabled;
                if (isCaptured)
                    CaptureAndLog($"Retrieved web element's 'Enabled' property [{FindType:g} : {ValueToFind}] - Property value = '{isEnabled}'");//'{GetTextOrValue()}'");
            });
            return isEnabled;
        }

        /// <summary>
        /// Verified the element displayed on this screen and log the information
        /// </summary>
        /// <returns>displayed or not</returns>
        public bool IsDisplayed(bool isCaptured = true)
        {
            if (!IsExisted(isCaptured))
                return false;

            //Log.Debug($"Retrieving web element's 'Displayed' property [{FindType:g} : {ValueToFind}]");
            bool isDisplayed = false;
            ActionWithTryCatch(() =>
            {
                IWebElement element = GetWrappedControl();

                bool js_visible = false;
                try { js_visible = driver.ExecuteJavaScript<bool>($"window.getComputedStyle({element}).visibility !== 'hidden';"); } catch { }

                if (element.Displayed || UtilsHelper.is_valid(element) || js_visible)
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

        public string GetText(bool isCaptured = true)
        {
            //Log.Debug($"Retrieving web element's 'Text' property [{FindType:g} : {ValueToFind}]");
            string text = string.Empty;
            ActionWithTryCatch(() =>
            {
                text = GetWrappedControl().Text;
                if (isCaptured)
                    CaptureAndLog($"Retrieved web element's 'Text' property [{FindType:g} : {ValueToFind}] - Property value = '{text}'");//'{GetTextOrValue()}'");
            });
            return text;
        }

        public string GetValue(bool isCaptured = true)
        {
            return GetAttribute("value", isCaptured);
        }

        public Point GetLocation(bool isCaptured = true)
        {
            //Log.Debug($"Retrieving web element's 'Location' property [{FindType:g} : {ValueToFind}]");
            Point location = new Point();
            ActionWithTryCatch(() =>
            {
                location = GetWrappedControl().Location;
                if (isCaptured)
                    CaptureAndLog($"Retrieved web element's 'Location' property [{FindType:g} : {ValueToFind}] - Property value = 'X = {location.X} | Y = {location.Y}'");//'{GetTextOrValue()}'");
            });
            return location;
        }

        public Size GetSize(bool isCaptured = true)
        {
            //Log.Debug($"Retrieving web element's 'Size' property [{FindType:g} : {ValueToFind}]");
            Size size = new Size();
            ActionWithTryCatch(() =>
            {
                size = GetWrappedControl().Size;
                if (isCaptured)
                    CaptureAndLog($"Retrieved web element's 'Size' property [{FindType:g} : {ValueToFind}] - Property value = 'Width = {size.Width} | Height = {size.Height}'");//'{GetTextOrValue()}'");
            });
            return size;
        }

        public bool WaitUntilExist(int timeout)
        {
            this.wrappedControl = FindElementHelper.FindElement(this.FindType, this.ValueToFind, timeout);
            if (this.wrappedControl != null)
                return true;
            return false;
        }

        public bool WaitForElementIsVisible(int timeout, bool captureAndLog = true)
        {
            //UtilsHelper.debug_output($"Waiting for web element [{FindType:g} : {ValueToFind}] to become visible within {timeout} seconds...");

            IWebElement element;

            wait = new WebDriverWait(BaseValues.DriverSession, TimeSpan.FromSeconds(timeout));
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
                    CaptureAndLog($"The web element <b>[{FindType:g} : {ValueToFind}]</b> is <font color='green'><b>visible</b></font>.");

                return element.Displayed;
            }
            catch (Exception e)
            {
                //UtilsHelper.debug_output($"Encountered exception while waiting for web element to become visible - Exception: ->\n" + e.StackTrace);
                return false;
            }


            //catch (WebDriverTimeoutException e)
            //{
            //    Log.Warn($"The element [{FindType:g} : {ValueToFind}] does not exist after {timeout} seconds - " + e.Message);
            //    return false;
            //}
        }

        public bool WaitForElementIsInVisible(int timeout, bool captureAndLog = true)
        {
            //UtilsHelper.debug_output($"Waiting for web element [{FindType:g} : {ValueToFind}] to hide within {timeout} seconds...");

            wait = new WebDriverWait(BaseValues.DriverSession, TimeSpan.FromSeconds(timeout));
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
                    ExtentReportsHelper.LogInformation($"Web element <b>[{FindType:g} : {ValueToFind}]</b> is <font color='green'><b>successfully hidden</b></font>.");
                return hide;
            }
            catch (Exception e)
            {
                UtilsHelper.debug_output($"Encountered exception while waiting for web element to become hidden - Exception: ->\n" + e.StackTrace);
                return false;
            }

            //catch (WebDriverTimeoutException e)
            //{
            //    Log.Warn($"The element [{FindType:g} : {ValueToFind}] does not exist after {timeout} seconds - " + e.Message);
            //    if (captureAndLog)
            //        ExtentReportsHelper.LogInformation($"The element <font color='green'><i>[{ FindType:g} : { ValueToFind}]</i></font> is <b><i>NOT</i></b> hidden after { timeout} seconds");
            //    return false;
            //}
        }
        public void WaitForElementIsPresence(int timeout, bool captureAndLog = true)
        {
            //UtilsHelper.debug_output($"Waiting for web element [{FindType:g} : {ValueToFind}] to hide within {timeout} seconds...");

            wait = new WebDriverWait(BaseValues.DriverSession, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(WebDriverTimeoutException));
            try
            {
                //bool hide = false;
                switch (FindType)
                {
                    case FindType.Id:
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id(ValueToFind)));
                        break;
                    case FindType.Name:
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Name(ValueToFind)));
                        break;
                    case FindType.XPath:
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(ValueToFind)));
                        break;
                    case FindType.CssSelector:
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(ValueToFind)));
                        break;
                    default:
                        break;
                }
                if (captureAndLog)
                    ExtentReportsHelper.LogInformation($"Web element <b>[{FindType:g} : {ValueToFind}]</b> is <font color='green'><b>successfully hidden</b></font>.");
            }
            catch (Exception e)
            {
                UtilsHelper.debug_output($"Encountered exception while waiting for web element to become hidden - Exception: ->\n" + e.StackTrace);
            }

            //catch (WebDriverTimeoutException e)
            //{
            //    Log.Warn($"The element [{FindType:g} : {ValueToFind}] does not exist after {timeout} seconds - " + e.Message);
            //    if (captureAndLog)
            //        ExtentReportsHelper.LogInformation($"The element <font color='green'><i>[{ FindType:g} : { ValueToFind}]</i></font> is <b><i>NOT</i></b> hidden after { timeout} seconds");
            //    return false;
            //}
        }


        /// <summary>
        /// Update the current value to find of this control to new value to find
        /// </summary>
        /// <param name="newValueToFind"></param>
        public void UpdateValueToFind(string newValueToFind)
        {
            this.ValueToFind = newValueToFind;
        }

        /// <summary>
        /// Update the current find type to new findtype
        /// </summary>
        /// <param name="newFindType"></param>
        public void UpdateFindType(FindType newFindType)
        {
            this.FindType = newFindType;
        }

        public void SetAttribute(string attName, string attValue, bool isCaptured)
        {
            ActionWithTryCatch(() =>
            {
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                if (isCaptured)
                    CaptureAndLog($"This control is set to attribute <font color='green'><b>{attName}</b></font> with value <font color='green'><b>{attValue}</b></font>.");
                executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);",
                        wrappedControl, attName, attValue);
            });
        }

        /// <summary>
        /// Hover mouse on the element
        /// </summary>
        public void HoverMouse(bool isCaptured = true)
        {
            ActionWithTryCatch(() =>
            {
                Actions action = new Actions(driver);
                action.MoveToElement(GetWrappedControl()).Perform();
                if (isCaptured)
                    CaptureAndLog($"Hovered on web element <font color='green'><i>'{GetTextOrValue()}'</i></font>.");
            });
        }

        /// <summary>
        /// Click at the element and using javascript
        /// </summary>
        public void JavaScriptClick(bool isCaptured = true)
        {
            ActionWithTryCatch(() =>
            {
                UtilsHelper.MoveToElement(GetWrappedControl());

                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                if (isCaptured)
                    CaptureAndLog($"Element <font color ='green'><b>{GetTextOrValue()}</b></font> will be clicked after capturing the screenshot");

                executor.ExecuteScript("arguments[0].focus();", GetWrappedControl());

                System.Threading.Thread.Sleep(33);

                executor.ExecuteScript("arguments[0].click();", GetWrappedControl());
            });
        }

        /// <summary>
        /// Click at the element with coordinate
        /// </summary>
        public void CoordinateClick(bool isCaptured = true)
        {
            ActionWithTryCatch(() =>
            {
                Actions action = new Actions(driver);
                CaptureAndLog($"Element <font color ='green'><b>{GetTextOrValue()}</b></font> will be clicked after capturing the screenshot");
                if (isCaptured)
                    CaptureAndLog("Hovered on the element.");
                action.MoveToElement(GetWrappedControl()).Click().Build().Perform();
            });
        }

        public void ClickAtPosition(Margin margin, int percentValue, bool isCaptured = true)
        {
            ActionWithTryCatch(() =>
            {
                Size size = GetWrappedControl().Size;
                int x = size.Width;
                int y = size.Height;
                int expectedValue = 0;

                switch (margin)
                {
                    case Margin.Top:
                        y /= 2;
                        expectedValue = x * percentValue / 100;
                        if (expectedValue < x)
                            x = expectedValue;
                        break;
                    case Margin.Left:
                        x /= 2;
                        expectedValue = y * percentValue / 100;
                        if (expectedValue < y)
                            y = expectedValue;
                        break;
                    case Margin.Bottom:
                        y /= 2;
                        expectedValue = x * percentValue / 100;
                        if (expectedValue < x && x - expectedValue > 0)
                            x -= expectedValue;
                        break;
                    default:
                        x /= 2;
                        expectedValue = y * percentValue / 100;
                        if (expectedValue < y && y - expectedValue > 0)
                            y -= expectedValue;
                        break;
                }
                string text = GetTextOrValue();
                Actions builder = new Actions(BaseValues.DriverSession);
                if (isCaptured)
                    CaptureAndLog($"The page was clicked on the element <font color='green'><b><i>'{text}'</i></b></font> at position <b><i>x:{x}|y:{y}</i></b>.");
                builder.MoveToElement(GetWrappedControl(), x, y).Click().Build().Perform();
            });
        }

        protected void ActionWithTryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (StaleElementReferenceException e)
            {
                Log.Error($"The element [{FindType:g}|{ValueToFind}] is out of date - " + e.Message);
                throw new StaleElementReferenceException("The element is out of date");
            }
            catch (ElementNotInteractableException e)
            {
                Log.Error($"The element [{FindType:g}|{ValueToFind}] is not interactable - " + e.Message);
                throw new ElementNotInteractableException("The element is not interactable (hidden or disabled...)");
            }
            catch (NoSuchElementException e)
            {
                UtilsHelper.debug_output($"The element [{FindType:g}|{ValueToFind}] does not exist on DOM - " + e.Message);
                throw new NoSuchElementException("The element could not be found");
            }
        }

        protected TResult ReturnActionResultWithTryCatch<TResult>(Func<TResult> action)
        {
            try
            {
                return action();
            }
            catch (StaleElementReferenceException e)
            {
                Log.Error($"The element [{FindType:g}|{ValueToFind}] is out of date - " + e.Message);
                throw new StaleElementReferenceException("The element is out of date");
            }
            catch (ElementNotInteractableException e)
            {
                Log.Error($"The element [{FindType:g}|{ValueToFind}] is not interactable - " + e.Message);
                throw new ElementNotInteractableException("The element is not interactable (hidden or disabled...)");
            }
            catch (NoSuchElementException e)
            {
                UtilsHelper.debug_output($"The element [{FindType:g}|{ValueToFind}] does not exist on DOM - " + e.Message);
                throw new NoSuchElementException("The element could not be found");
            }
        }

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
        */
    }
}
