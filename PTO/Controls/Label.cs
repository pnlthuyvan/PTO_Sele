using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;

namespace PTO.Controls
{
    public class Label : BaseControl
    {
        public Label(IWebDriver driverTest) : base(driverTest) { driver = driverTest; }
        public Label(IWebDriver driverTest, FindType findType, string valueToFind) : base(driverTest, findType, valueToFind) { }
    }
}
