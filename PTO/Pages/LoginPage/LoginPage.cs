using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PTO.Base;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Pages.LoginPage
{
    public class LoginPage(IWebDriver driver)
    {
        private IWebDriver driverTest = driver;

        public void SignIn()
        {
            var yourUsername = BaseValues.UserName;
            var yourPassword = BaseValues.Password;

            if(string.IsNullOrEmpty(yourUsername) || string.IsNullOrEmpty(yourPassword))
            {
                Assert.Fail($"No account from config file to sign in.");
            }

            SignIn(yourUsername, yourPassword);

            // Assert: Check if the navigation to the login page was successful
            if (!string.IsNullOrEmpty(driverTest.Title) && driverTest.Title == "Pipeline Takeoff")
                ExtentReportsHelper.LogPassAndCap(driverTest, $"Title is same as expected: 'Pipeline Takeoff'");
            else
                ExtentReportsHelper.LogFailAndCap(driverTest, $"Title is NOT same as expected. Actual: '{driverTest.Title}'");


            // Additional assertions or test logic
            if (IsLoggedIn())
                ExtentReportsHelper.LogPassAndCap(driverTest,$"Login succesfully.");
            else
                ExtentReportsHelper.LogFailAndCap(driverTest,$"Failed to login.");
        }

        /// <summary>
        /// Verify if login successfully
        /// </summary>
        /// <param name="driverTest"></param>
        /// <returns></returns>
        private bool IsLoggedIn()
        {
            // Use WebDriverWait to wait for the page to be fully loaded
            var wait = new WebDriverWait(driverTest, TimeSpan.FromSeconds(10));
            try
            {
                // Wait until the document.readyState is 'complete'
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Add additional conditions if needed, e.g., checking for specific elements indicating login
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false; // Return false if the timeout is reached
            }
        }

        /// <summary>
        /// Sign in with username and pass from app config file
        /// </summary>
        /// <param name="Driver"></param>
        /// <param name="email"></param>
        /// <param name="passs"></param>
        /// <returns></returns>
        public IWebDriver SignIn(string email, string passs)
        {
            try
            {
                ExtentReportsHelper.LogInformationAndCap(driverTest, $"Attempting AutoSSO...");

                // Input username
                var userNameTxt = driverTest.FindElement(By.XPath($"//input[contains(@name, 'Email')]"));
                var loginBtn = driverTest.FindElement(By.XPath("//*[contains(@class, 'ssobutton ssoprimarybutton')]"));

                userNameTxt.Clear();
                userNameTxt.SendKeys(email);

                loginBtn.Click();
                UtilsHelper.WaitPageLoad(driverTest);

                // Input password
                var passwordTxt = driverTest.FindElement(By.XPath($"//input[@id='Password']"));
                if (passwordTxt != null && passwordTxt.Displayed)
                {
                    passwordTxt.Clear();
                    passwordTxt.SendKeys(passs);
                }

                // Click Signin
                loginBtn = driverTest.FindElement(By.XPath("//*[contains(@class, 'ssobutton ssoprimarybutton')]"));
                if (loginBtn != null && loginBtn.Displayed)
                {
                    loginBtn.Click();
                    UtilsHelper.WaitPageLoad(driverTest);
                }
                return driverTest;
            }
            catch (Exception exception)
            {
                Assert.Fail($"Failed to detect web elements for AutoSSO, elements may have changed or an unexpected page is displayed - Exception: ->\n{exception.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public void Logout()
        {
            try
            {
                // UtilsHelper.test_output("Logging out of web application...", false);

                // Click user Profile/ select Log Out button
                var userProfile_btn = driverTest.FindElement(By.XPath("//*[@id='CurrentUserProfileImage']"));
                if (userProfile_btn.Displayed is false)
                {

                    //UtilsHelper.test_output($"<font color = 'red'>Failed to detect the User Profile menu to log out.</font>'");
                    return;
                }
                userProfile_btn.Click();

                // Click Log out button on User Profile panel
                string logOut_Xpath = "//*[@id='signoutForm']/button";
                var logOut_btn = driverTest.FindElement(By.XPath(logOut_Xpath));

                if (logOut_btn.Displayed is false)
                {
                    //UtilsHelper.test_output($"<font color = 'red'>Failed to detect the Log out button on User Profile panel to log out.</font>'");
                    return;
                }
                logOut_btn.Click();

                UtilsHelper.WaitPageLoad(driverTest);

                // Wait until the confirm dialog display
                string confirm_Xpath = "//*[@id='Confirm']";
                UtilsHelper.WaitForElementIsVisible(driverTest, FindType.XPath, confirm_Xpath, 1000);

                var confirmLogOutButton = driverTest.FindElement(By.XPath(confirm_Xpath));
                if (confirmLogOutButton.Displayed is false)
                {
                    //UtilsHelper.test_output($"<font color = 'red'>Failed to detect the confirm Log out button on 'Are you sure you want to sign out of all applications?' page.</font>'");
                    return;
                }
                confirmLogOutButton.Click();
                UtilsHelper.WaitPageLoad(driverTest);

                // Wait until the login page display
                UtilsHelper.WaitForElementIsVisible(driverTest, FindType.XPath, "//*[@id='formLogin']", 1000);
                driverTest.Close();
            }
            catch (Exception exception)
            {
                //UtilsHelper.test_output($"Failed to detect web elements for AutoSSO, elements may have changed or an unexpected page is displayed - Exception: ->\n{exception.StackTrace}");
                driverTest.Close();
            }

        }
    }
}
