using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Controls
{
    public class TextBox : BaseControl
    {
        private new readonly IWebDriver driver;
        public TextBox(IWebDriver driverTest) : base(driverTest) { driver = driverTest; }
        public TextBox(IWebDriver driverTest, FindType findType, string valueToFind) : base(driverTest, findType, valueToFind) { }

        /// <summary>
        /// Set value to textbox
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCaptured"></param>
        public void SendKeys(string value, bool isCaptured = true)
        {
            UtilsHelper.ActionWithTryCatch(() =>
            {
                string message = $"Input value <font color='green'><b><i>'{value}'</i></b></font> to field <font color='green'><b>'{GetWrappedControl().TagName}'</b></font>.";
                UtilsHelper.DebugOutput($"Sending value '{value}' to web element [{FindType:g} | {ValueToFind}]");

                GetWrappedControl().Clear();
                GetWrappedControl().SendKeys(value);
                if (isCaptured)
                    CaptureAndLog(message);
            });
        }
    }
}
