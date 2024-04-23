using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using PTO.Constants;
using PTO.Utilities;
using System.Text.RegularExpressions;

namespace PTO.Pages.Demo
{
    public class DemoPage (IWebDriver driverTest)
    {
        private readonly string loadingIcon = "//*[@id='LoadingModal']/div";
        private readonly IWebDriver driver = driverTest;
        private readonly int waitingTime = 20;


        /// <summary>
        /// Open an existing job
        /// </summary>
        /// <param name="selectedJobNum"></param>
        public void OpenJob(string selectedJobNum)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='lavender'><b>Step 1: Open Job.</b></font>");


                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitingTime);

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
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver,$"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
            ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'>Open Job successfully.</font>");

        }

        /// <summary>
        /// Select a sheet after opening a job
        /// </summary>
        /// <param name="sheetName"></param>
        public void SelectSheet(string sheetName)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='lavender'><b>Step 2: Open Sheet.</b></font>");

                driver.FindElement(By.XPath($"//span[@title = '{sheetName}' and not(contains(@data-bind, 'style'))]")).Click();

                // Wait until rendering succesfully
                System.Threading.Thread.Sleep(8000);
                var renderingXpath = "//span{text() = 'Rendering...']";

                // Wait to display
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, renderingXpath);

                // Wait to disappear
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, renderingXpath);
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not open an existing sheet '{sheetName}'. There is an error exception: {e.Message}</font>");
            }
            ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'>Select Sheet successfully.</font>");

        }

        /// <summary>
        /// Select Key Measure on Uncategorized
        /// </summary>
        /// <param name="keyMeasure"></param>
        public void SelectKeyMeasure(string keyMeasure)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='lavender'><b>Step 4: Select KeyMeasure from Uncategozied.</b></font>");

                // Select Categoey
                driver.FindElement(By.XPath($"//span[text() = 'Uncategorized']")).Click();
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, loadingIcon);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                // Wait until KeyMeasure displays
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, $"//span[text() = '{keyMeasure}']");

                // Select Key Measure
                var keyMeasureName = driver.FindElement(By.XPath("//span[text() = '{keyMeasure}']"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", keyMeasureName);
                //driver.FindElement(By.XPath($"//span[text() = '{keyMeasure}']")).Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitingTime);

                // Wait until modal displays
                string modalXpath = "//div[@id= 'preTakeoffModal']//h2[text() = 'Measurement Properties']";
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, modalXpath);

                // Click Apply
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, "//div[@id= 'preTakeoffModal']//button[text()='Apply']");
                driver.FindElement(By.XPath($"//div[@id= 'preTakeoffModal']//button[text()='Apply']")).Click();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
            ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'>Select Key Measure successfully.</font>");

        }

        /// <summary>
        /// Zoom the drawing
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void ZoomInOut(int zoomLevel = 70, int left = 500, int top = 200)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='lavender'><b>Step 3: Zoom in.</b></font>");

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

                // Click on slider, it will move and zoom 50% first

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                builder.ClickAndHold(ZoomSlider)
                       .MoveByOffset(0, 10) // (x, y) (adjust if needed)
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
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not zoom sheet. There is an error exception: {e.Message}</font>");

            }
            ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'><b>Zoom in successfully.</b></font>");

        }

        /// <summary>
        /// Draw by linear and calculate it
        /// </summary>
        public void Linear()
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='lavender'><b>Step 5: Draw the linear.</b></font>");

                IWebElement image = driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"));

                Actions action = new(driver);

                   action
                           .MoveToElement(image, 20, 100)
                           .Click()
                           .Perform();

                   driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitingTime);

                   action
                      .MoveToElement(image, 20, -120)
                      .Click()
                      .Perform();
                  

                // Click first coordinate
                //action.Click(driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"))).MoveByOffset(1, 90).Release().Build().Perform();
                //// Click second coordinate
                //action.Click(driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"))).MoveByOffset(60, 90).Release().Build().Perform();

               /* action.MoveByOffset(0, 100).Click()
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
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.CssSelector, loadingIcon);

                // Wait until toast message close
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.CssSelector, "//*[@id='LoadingModal']");


                float afterDrawTakeoffValue = float.Parse(driver.FindElement(By.XPath(calculatedValueSelector)).Text);

                // If after = before + calculation => correct
                float router = (float)Math.Round(beforeDrawTakeoffValue + calculatedValue, 2);
                if (afterDrawTakeoffValue == router)
                {
                    ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, "<font color='green'><b>Calculation is CORRECT</font>");
                }
                else
                {
                    ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, "<font color='red'>Calculation is INCORRECT" +
                        $"<br>Expected result: {afterDrawTakeoffValue}" +
                        $"<br>Actual result: {beforeDrawTakeoffValue + calculatedValue}</br></font>");
                }
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
        }

        /// <summary>
        /// Calculate quantity of measure
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private float CalculateMeasurementMetric(string inputString)
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
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>There is an error while calculate the quantity.</font>");
                return 0;
            }
        }
    }
}
