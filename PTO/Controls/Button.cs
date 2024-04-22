using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Controls
{
    internal class Button : BaseControl
    {
        private new readonly IWebDriver driver;
        public Button(IWebDriver driverTest) : base(driverTest) { this.driver = driverTest; }
        public Button(IWebDriver driverTest, FindType findType, string valueToFind) : base(driverTest, findType, valueToFind) { }


        /// <summary>
        /// Click function
        /// </summary>
        /// <param name="isCapture"></param>
        public void Click(bool isCapture = true)
        {
            UtilsHelper.ActionWithTryCatch(() =>
            {
                if (isCapture)
                    CaptureAndLog($"Clicking web element <font color ='green'><b><i>{GetTextOrValue()}</i></b></font>...");
                GetWrappedControl().Click();
            });
        }

        public void ClickByJavascript(bool isCapture = true)
        {
            UtilsHelper.JavaScriptClick(driver, this, isCapture);
        }
    }
}