using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using PTO.Constants;
using PTO.Controls;
using PTO.Utilities;
using System.Text.RegularExpressions;

namespace PTO.Pages.TakeoffPage
{
    public partial class TakeoffPage
    {
        private readonly string loadingIcon = "//*[@id='LoadingModal']/div";

        #region "Open Job"
        /// <summary>
        /// Open an existing job
        /// </summary>
        /// <param name="selectedJobNum"></param>
        public void OpenJob(string selectedJobNum)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='Magenta '><b>Step 1: Open Job.</b></font>");

                // Click Open Job button
                OpenJob_btn.Click();

                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                // Wait until job number display
                var jobNumber = $"//td[@data-field='Name' and contains(text(), '{selectedJobNum}')]";
                CommonElement jobNumber_lbl = new(driver, FindType.XPath, jobNumber);
                if (!jobNumber_lbl.IsExisted())
                {
                    ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Can't find Job with number: '{selectedJobNum}' to select.</font>");
                    return;
                }

                // Click Job Number on the grid view
                Button selectedJobButton = new(driver, FindType.XPath, jobNumber);
                selectedJobButton.WaitForElementIsVisible();
                selectedJobButton.Click();

                // Wait loading icon
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                // Click Open button on the modal
                Open_btn.Click();

                // Wait until job displays
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                JQueryLoad();

                ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'><b>Open Job successfully.</b></font>");
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
        }
        #endregion

        #region "Sheet"
        /// <summary>
        /// Select a sheet after opening a job
        /// </summary>
        /// <param name="sheetName"></param>
        public void SelectSheet(string sheetName)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='Magenta '><b>Step 2: Open Sheet.</b></font>");

                Button selectedSheet_btn = new(driver, FindType.XPath, $"//span[@title = '{sheetName}' and not(contains(@data-bind, 'style'))]");
                selectedSheet_btn.WaitForElementIsVisible();
                selectedSheet_btn.Click();

                // Wait until rendering succesfully
                WaitForLoadingAnimation();
                var renderingXpath = "//span[contains(text(), 'Rendering...')]";

                // Wait to display
                UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, renderingXpath);

                // Wait to disappear
                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, renderingXpath);

                ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'><b>Select Sheet successfully.</b></font>");
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could not open an existing sheet '{sheetName}'. There is an error exception: {e.Message}</font>");
            }
        }
        #endregion

        #region "Key Measure"
        /// <summary>
        /// Select Key Measure on Uncategorized
        /// </summary>
        /// <param name="keyMeasure"></param>
        public void SelectKeyMeasure(string keyMeasure)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='Magenta'><b>Step 4: Select KeyMeasure from Uncategozied.</b></font>");

                // Select Category
                Button selectedCategory_btn = new(driver, FindType.XPath, $"//span[text() = 'Uncategorized']");
                selectedCategory_btn.WaitForElementIsVisible();
                selectedCategory_btn.Click();

                // Select Key Measure
                Button selectedKM_btn = new(driver, FindType.XPath, $"//span[text() = '{keyMeasure}']");
                selectedKM_btn.WaitForElementIsVisible();
                selectedKM_btn.Click();

                // Wait until modal displays
                string modalXpath = "//div[@id= 'preTakeoffModal']//h2[text() = 'Measurement Properties']";
                CommonElement kmModal_lbl = new(driver, FindType.XPath, modalXpath);
                if (!kmModal_lbl.IsExisted())
                {
                    ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Can't find 'Measurement Properties' modal with selected KM: '{keyMeasure}' to draw.</font>");
                    return;
                }

                // Click Apply
                var applyXpath = "//div[@id= 'preTakeoffModal']//button[text()='Apply']";
                Button apply_btn = new(driver, FindType.XPath, applyXpath);
                apply_btn.WaitForElementIsVisible();
                apply_btn.Click();

                ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'><b>Select Key Measure successfully.</b></font>");
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could NOT select Key Measure. There is an error exception: {e.Message}</font>");
            }
        }
        #endregion

        #region "Draw"

        /// <summary>
        /// Draw by linear and calculate it
        /// </summary>
        public void Linear(string keymeasureName)
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

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
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

                //Get value ... Ft next to the product name - HOVER TO GET IT
                string calculatedValueSelector = $"//div[text() = '{keymeasureName}']/following-sibling::div[1]";
                CommonElement beforeDrawTakeoffElement_lbl = new(driver, FindType.XPath, calculatedValueSelector);
                beforeDrawTakeoffElement_lbl.WaitForElementIsVisible();
                string originalValue = beforeDrawTakeoffElement_lbl.GetAttribute("title");
                float beforeDrawTakeoffValue = float.Parse(originalValue);

                //Get the red text (raw value from drawing action)
                string redValueSelector = "#takeoff-svg-layer-takeoff-tab g.drawing-takeoff text";
                CommonElement redValueSelector_lbl = new(driver, FindType.CssSelector, redValueSelector);
                redValueSelector_lbl.WaitForElementIsVisible();
                string redValue = redValueSelector_lbl.GetTextOrValue();
                float calculatedValue = CalculateMeasurementMetric(redValue);

                // Click stop record
                StopRecord_btn.Click();

                // Verify toast message
                string expectedMessTitle = "Successfully Measured Item";
                string expectedMessContent = $"{keymeasureName} has been measured successfully.";
                VerifyToastMessage(expectedMessTitle, expectedMessContent);


                float afterDrawTakeoffValue = float.Parse(driver.FindElement(By.XPath(calculatedValueSelector)).Text);

                // If after = before + calculation => correct
                float a = beforeDrawTakeoffValue + calculatedValue;
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
        private static float CalculateMeasurementMetric(string inputString)
        {
            // Split the string using the regular expression /[\"\-\s]+/
            string pattern = @"(\d+)'\s*(\d+)""\s*-?\s*(\d+/?\d*)";

            // Extract the numeric values from the array parts
            Match match = Regex.Match(inputString, pattern);

            if (match.Success)
            {
                float feet = float.Parse(match.Groups[1].Value); // 29
                float inches = float.Parse(match.Groups[2].Value); // 2
                string fractionBeforeConvert = match.Groups[3].Value; // 1/2
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
                    fraction = float.Parse(fractionBeforeConvert) / 16;
                }

                // Perform the calculation
                return feet + ((inches + fraction) / 12);
            }
            else
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>There is an error while calculate the quantity.</font>");
                return 0;
            }
        }

        #endregion

        #region "Work area"
        /// <summary>
        /// Zoom the drawing
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void ZoomInOut(int zoomLevel = 10, int left = 500, int top = 200)
        {
            try
            {
                ExtentReportsHelper.LogInformation("<font color='Magenta'><b>Step 3: Zoom in.</b></font>");

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Execute JavaScript to set the zoom level
                IWebElement ZoomSlider = driver.FindElement(By.CssSelector(".card-body-middle .zoom-slider"));

                ZoomSlider.Click();

                /*Button ZoomSlider_btn = new(driver, FindType.CssSelector, ".card-body-middle .zoom-slider");
                ZoomSlider_btn.WaitForElementIsVisible();
                ZoomSlider_btn.Click();
                */

                // Click on slider, it will move and zoom 50% first
                Actions builder = new(driver);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                builder.ClickAndHold(ZoomSlider)
                       .MoveByOffset(0, zoomLevel) // (x, y) (adjust if needed)
                       .Release()
                       .Perform();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                // Execute JavaScript to scroll to the desired section
                IWebElement SectionView = driver.FindElement(By.CssSelector("#takeoff-container-takeoff-tab"));

                // Scroll horizontally
                js.ExecuteScript($"arguments[0].scrollLeft = {left};", SectionView);
                // Scroll vertically
                js.ExecuteScript($"arguments[0].scrollTop = {top};", SectionView);

                ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color = 'green'><b>Zoom in successfully.</b></font>");
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color = 'red'>Could NOT zoom in sheet. There is an error exception: {e.Message}</font>");
            }
        }
        #endregion

    }
}
