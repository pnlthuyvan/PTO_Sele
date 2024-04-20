using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;

namespace PTO.Controls
{
    public class CommonElement : BaseControl
    {
        public CommonElement(IWebDriver driver) : base(driver) { }
        public CommonElement(IWebDriver driver, FindType findType, string valueToFind) : base(driver, findType, valueToFind) { }
    }
}
