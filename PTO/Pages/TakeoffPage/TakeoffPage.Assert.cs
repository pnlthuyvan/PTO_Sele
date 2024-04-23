using PTO.Base;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Pages.TakeoffPage
{
    public partial class TakeoffPage
    {
        #region "Draw"

        /// <summary>
        /// Verify page displays as expected
        /// </summary>
        /// <returns></returns>
        public bool IsTakeoffPageDisplayed()
        {
            if (GetHeaderText().Equals("[No Job Selected]") 
                && CurrentURL.StartsWith(BaseValues.BaseURL) 
                && CurrentTitle.Equals(BaseConstants.APP_PIPELINE_TAKEOFF))
            {
                ExtentReportsHelper.LogPass($"<font color='green'><b>The Takeoff page displayed successfully.</b></font>");
                return true;
            }
            else
                ExtentReportsHelper.LogFail($"<font color='red'>The Takeoff page failed to display.</font>" +
                    $"<br>Current URL = " + CurrentURL);
            return false;
        }

        /// <summary>
        /// Verify toast message after creating a item
        /// </summary>
        /// <param name="expectedMessTitle"></param>
        /// <param name="expectedMessContent"></param>
        public void VerifyToastMessage(string expectedMessTitle, string expectedMessContent)
        {
            string actualMessTitle = GetToastMeassage(BaseConstants.TOAST_MESSAGE_TITLE);
            string actualMess = GetToastMeassage(BaseConstants.TOAST_MESSAGE_CONTENT);

            if (!expectedMessTitle.Equals(actualMessTitle) || !expectedMessContent.Equals(actualMess))
                ExtentReportsHelper.LogFailAndCaptureFullScreen(driver, $"<font color='red'><b>The message isn't same as expected." +
                    $"<br>Expected title: {expectedMessTitle}" +
                    $"<br>Expected content: {expectedMessContent}" +
                    $"<br>Actual title: {actualMessTitle}" +
                    $"<br>Actual content: {actualMess}</br></font>");
            else
                ExtentReportsHelper.LogPassAndCaptureFullScreen(driver, $"<font color='green'><b>Save successfully. Toast message is same as expected.</b></font>");
        }


        #endregion
    }
}
