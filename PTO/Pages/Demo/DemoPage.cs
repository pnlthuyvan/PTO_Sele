using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using PTO.Constants;
using PTO.Utilities;
using Serilog;
using System.Text.RegularExpressions;

namespace PTO.Pages.Demo
{
    public class DemoPage (IWebDriver driverTest)
    {
        private static readonly ILogger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        private readonly string loadingIcon = "//*[@id='LoadingModal']/div";

        private IWebDriver driver = driverTest;


        /// <summary>
        /// Open an existing job
        /// </summary>
        /// <param name="selectedJobNum"></param>
        public void OpenJob(string selectedJobNum)
        {
            try
            {
                ExtentReportsHelper.LogInformation($"start job");

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                // Click Open Job button
                driver.FindElements(By.XPath("//button[text() = 'Open Job']")).FirstOrDefault().Click();

                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                // Wait until job number display
                var jobNumber = $"//td[@data-field='Name' and contains(text(), '{selectedJobNum}')]";
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, jobNumber);

                // Click Job Number on the grid view
                IWebElement selectedJobButton = driver.FindElement(By.XPath(jobNumber));
                selectedJobButton.Click();

                // Wait loading icon
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                // Click Open button on the modal
                driver.FindElements(By.XPath($"//button[text()='Open']")).FirstOrDefault().Click();

                // Wait until job displays
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCap(driver,$"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
            ExtentReportsHelper.LogPassAndCap(driver, $"<font color = 'green'>Open Job successfully.</font>");

        }

        /// <summary>
        /// Select a sheet after opening a job
        /// </summary>
        /// <param name="sheetName"></param>
        public void SelectSheet(string sheetName)
        {
            try
            {
                driver.FindElement(By.XPath($"//span[@title = '{sheetName}' and not(contains(@data-bind, 'style'))]")).Click();

                // Wait until rendering succesfully
                System.Threading.Thread.Sleep(8000);
                var renderingXpath = "//span{text() = 'Rendering...']";

                // Wait to display
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, renderingXpath, 20);

                // Wait to disappear
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, renderingXpath, 20);
            }
            catch (Exception e)
            {
                Assert.Fail($"<font color = 'yellow'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
        }

        /// <summary>
        /// Select Key Measure on Uncategorized
        /// </summary>
        /// <param name="keyMeasure"></param>
        public void SelectKeyMeasure(string keyMeasure)
        {
            try
            {
                // Select Categoey
                driver.FindElement(By.XPath($"//span[text() = 'Uncategorized']")).Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                // Wait until KeyMeasure displays
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, $"//span[text() = '{keyMeasure}']");

                // Select Key Measure
                driver.FindElement(By.XPath($"//span[text() = '{keyMeasure}']")).Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                // Wait until modal displays
                string modalXpath = "//div[@id= 'preTakeoffModal']//h2[text() = 'Measurement Properties']";
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, modalXpath);

                // Click Apply
                driver.FindElement(By.XPath($"//div[@id= 'preTakeoffModal']//button[text()='Apply']")).Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            }
            catch (Exception e)
            {
                Assert.Fail($"<font color = 'yellow'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
        }

        /// <summary>
        /// Zoom the drawing
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void ZoomInOut(int zoomLevel = 70, int left = 2000, int top = 1100)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Execute JavaScript to set the zoom level
                IWebElement ZoomSlider = driver.FindElement(By.CssSelector(".card-body-middle .zoom-slider"));

                //IWebElement ZoomSlider1 = driver.FindElement(By.XPath("//*[contains(@class, 'card-body-middle')]//input[@class='zoom-slider']"));
                //ZoomSlider1.SendKeys('60');
                //js.ExecuteScript($"arguments[0].value = '{zoomLevel}';", ZoomSlider1);
                //js.ExecuteScript("var a=document.querySelector('.card-body-middle .zoom-slider');a.value =75;a.dispatchEvent(new Event('input', { bubbles: true }));");
                //js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", ZoomSlider1);
                //js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", ZoomSlider);

                Actions builder = new Actions(driver);
                ZoomSlider.Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                builder.ClickAndHold(ZoomSlider)
                       .MoveByOffset(0, -20) // (x, y) (adjust if needed)
                       .Release()
                       .Perform();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                // Execute JavaScript to scroll to the desired section
                IWebElement SectionView = driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"));
                // Scroll horizontally
                js.ExecuteScript($"arguments[0].scrollLeft = {left};", SectionView);
                // Scroll vertically
                js.ExecuteScript($"arguments[0].scrollTop = {top};", SectionView);
            }
            catch (Exception e)
            {
                Assert.Fail($"<font color = 'yellow'>Could not zoom sheet. There is an error exception: {e.Message}</font>");

            }
        }

        /// <summary>
        /// Draw by linear and calculate it
        /// </summary>
        public void Linear()
        {
            try
            {
                IWebElement image = driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"));

                Actions action = new Actions(driver);

                action
                        .MoveToElement(image, 40, 100)
                        .Click()
                        .Perform();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                action
                   .MoveToElement(image, 40, 200)
                   .Click()
                   .Perform();

                // Click first coordinate
                //action.Click(driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"))).MoveByOffset(1, 90).Release().Build().Perform();
                //// Click second coordinate
                //action.Click(driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"))).MoveByOffset(60, 90).Release().Build().Perform();

                /*action.MoveByOffset(0, 100).Click()
                    .MoveByOffset(0, -100).Click()
                    .Perform();
                */

                //Get value ... Ft next to the product name
                string calculatedValueSelector = "//div[text() = 'BV_KM_2_Import']/following-sibling::div[1]";
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, calculatedValueSelector);
                IWebElement beforeDrawTakeoffElement = driver.FindElement(By.XPath(calculatedValueSelector));
                string test = beforeDrawTakeoffElement.Text;
                float beforeDrawTakeoffValue = float.Parse(test);

                //Get the red text (raw value from drawing action)
                string redValueSelector = "#takeoff-svg-layer-takeoff-tab g.drawing-takeoff text";
                UtilsHelper.WaitForElementIsVisible(driver, FindType.CssSelector, redValueSelector);
                IWebElement inputValue = driver.FindElement(By.CssSelector(redValueSelector));
                float calculatedValue = CalculateMeasurementMetric(inputValue.Text);

                // Click stop record
                driver.FindElement(By.XPath($"//button[@title='Stop Recording']")).Click();

                float afterDrawTakeoffValue = float.Parse(driver.FindElement(By.XPath(calculatedValueSelector)).Text);

                // If after = before + calculation => correct
                if (afterDrawTakeoffValue == beforeDrawTakeoffValue + calculatedValue)
                {
                    ExtentReportsHelper.LogPassAndCap(driver, "Calculation is CORRECT");
                }
                else
                {
                    ExtentReportsHelper.LogFailAndCap(driver, "Calculation is INCORRECT");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"<font color = 'yellow'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
        }

        /// <summary>
        /// Calculate quantity of measure
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static float CalculateMeasurementMetric(string inputString)
        {
            // Split the string using the regular expression /[\"\-\s]+/
            string pattern = @"(\d+)'\s*(\d+)""\s*-?\s*(\d+/?\d*)";

            // Extract the numeric values from the array parts
            Match match = Regex.Match(inputString, pattern);

            if (match.Success)
            {
                float feet = float.Parse(match.Groups[1].Value);
                float inches = float.Parse(match.Groups[2].Value);
                string fractionBeforeConvert = match.Groups[3].Value;
                float fraction;

                if (fractionBeforeConvert.Contains('/'))
                {
                    string[] parts = fractionBeforeConvert.Split('/');
                    int numerator = int.Parse(parts[0]);
                    int denominator = int.Parse(parts[1]);
                    fraction = (float)numerator / denominator;

                }
                else
                {
                    fraction = float.Parse(fractionBeforeConvert);
                }

                // Perform the calculation
                return feet + (inches / 12) + (fraction / 16);
            }
            else
            {
                Console.WriteLine("Pattern not matched.");
                return 0;
            }
        }
    }
}
