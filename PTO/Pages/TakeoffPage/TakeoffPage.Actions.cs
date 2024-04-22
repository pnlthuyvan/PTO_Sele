using PTO.Constants;
using PTO.Controls;
using PTO.Utilities;

namespace PTO.Pages.TakeoffPage
{
    public partial class TakeoffPage
    {
        private readonly string loadingIcon = "//*[@id='LoadingModal']/div";

        /// <summary>
        /// Open an existing job
        /// </summary>
        /// <param name="selectedJobNum"></param>
        public void OpenJob(string selectedJobNum)
        {
            try
            {
                ExtentReportsHelper.LogInformation(null, "<font color='lavender'><b>Step 1: Open Job.</b></font>");

                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitingTime);

                // Click Open Job button
                OpenJob_btn.Click();

                UtilsHelper.WaitForElementIsInVisible(driver, FindType.XPath, loadingIcon);

                // Wait until job number display
                var jobNumber = $"//td[@data-field='Name' and contains(text(), '{selectedJobNum}')]";
                //UtilsHelper.WaitForElementIsVisible(driver, FindType.XPath, jobNumber);
                Label jobNumber_lbl = new(driver, FindType.XPath, jobNumber);
                if (!jobNumber_lbl.IsExisted())
                {
                    ExtentReportsHelper.LogFailAndCap(driver, $"<font color = 'red'>Can't find Job with number: '{selectedJobNum}' to select.</font>");
                    return;
                }

                // Click Job Number on the grid view
                // IWebElement selectedJobButton = driver.FindElement(By.XPath(jobNumber));
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
            }
            catch (Exception e)
            {
                ExtentReportsHelper.LogFailAndCap(driver, $"<font color = 'red'>Could not open an existing job. There is an error exception: {e.Message}</font>");
            }
            ExtentReportsHelper.LogPassAndCap(driver, $"<font color = 'green'>Open Job successfully.</font>");

        }
    }
}
