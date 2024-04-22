using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;
using PTO.Controls;

namespace PTO.Pages.TakeoffPage
{
    public partial class TakeoffPage(IWebDriver driverTest) : BasePage(driverTest)
    {
        protected static IWebDriver driver;
        private static readonly Lazy<TakeoffPage> _lazy = new Lazy<TakeoffPage>(() => new TakeoffPage(driver));

        public static TakeoffPage Instance(IWebDriver driverTest)
        {
            driver = driverTest;
            return _lazy.Value;
        }

        #region "Open Job"
        private readonly Button OpenJob_btn = new(driver, FindType.XPath, "//button[text() = 'Open Job']");
        private readonly Button Open_btn = new(driver, FindType.XPath, "//button[text()='Open']");
        #endregion

        #region "Sheet"
        #endregion

        #region "Key Measure"
        #endregion

        #region "Work Area"
        #endregion
    }
}
