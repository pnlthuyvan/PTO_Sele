using AventStack.ExtentReports;
using OpenQA.Selenium;
using PTO.Base;
using System.Runtime.CompilerServices;

namespace PTO.Utilities
{
    public static class ExtentReportsHelper
    {
        private readonly static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ThreadStatic]
        private static ExtentTest? _parentTest;

        [ThreadStatic]
        private static List<ExtentTest>? _listOfParentsTest;

        [ThreadStatic]
        private static List<ExtentTest>? _listOfChildTest;

        [ThreadStatic]
        private static ExtentTest? _childTest;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest GetTest() { return _childTest; }

        #region "Create/ Update test report"

        public static bool IsParentTestNull
        {
            get
            {
                log.Debug("Parent test uninitialized");
                return _parentTest == null;
            }
        }

        /// <summary>
        /// Switching test set by Name
        /// </summary>
        /// <param name="testSetName"></param>
        public static void SwitchTestSet(string testSetName)
        {
            bool isFound = false;
            log.Debug($"Switching test set with name '{testSetName}'...");
            if (_listOfParentsTest is null)
            {
                log.Debug($"Generating list of parent tests...");
                _listOfParentsTest = new List<ExtentTest>();
            }
            foreach (var item in _listOfParentsTest)
            {
                if (item.Model.Name == testSetName)
                {
                    log.Debug($"Setting the parent test with name '{testSetName}'...");
                    _parentTest = item;
                    isFound = true;
                    break;
                }
            }
            if (!isFound)
            {
                log.Debug($"Could not find test set with name '{testSetName}' in the list, proceeding to create");
                CreateParentTest(testSetName);
            }
        }

        /// <summary>
        /// Create the Parent Test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest CreateParentTest(string testName, string description = "")
        {
            if (_listOfParentsTest == null)
            {
                log.Debug($"Generating list of parent tests...");
                _listOfParentsTest = new List<ExtentTest>();
            }
            log.Debug($"Creating parent test with name '{testName}' and description '{description}'...");
            _parentTest = BaseValues.GetExtentReports().CreateTest(testName, description);

            log.Debug($"Adding parent test with name '{testName}' and description '{description}' to list...");
            _listOfParentsTest.Add(_parentTest);
            return _parentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest CreateOrUpdateTest(string oldTestName, string newTestName, string description = "")
        {
            log.Debug($"Updating test name from '{oldTestName}' to '{newTestName}'...");
            if (_listOfChildTest.Where(p => p.Model.Name == oldTestName).Count() > 0)
            {
                _childTest.Model.Name = newTestName;
                _childTest.Model.Description = description;
            }
            else
                _childTest = CreateTest(newTestName, description);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest CreateTest(string testName, string description = "")
        {
            log.Debug($"Creating test with name '{testName}' and description '{description}'...");
            _childTest = _parentTest.CreateNode(testName, description);
            if (_listOfChildTest is null)
                _listOfChildTest = new List<ExtentTest>();
            _listOfChildTest.Add(_childTest);
            return _childTest;
        }



        #endregion

        #region "Log report and capture control only]

        public static ExtentTest LogFail(string details = "LOG", string? pathImg = null)
        {
            string msg = $"    *** FAILURE ***  ==  {details}";

            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);
            log.Debug(msg);

            if (IsChildTestNull) return null;

            if (pathImg != null)
                _childTest = _childTest.Fail(details, MediaEntityBuilder.CreateScreenCaptureFromPath(pathImg).Build());
            else
                _childTest = _childTest.Fail(details);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogWarning(string details = "LOG", string? pathImg = null)
        {
            string msg = $"    *** WARNING ***  ==  {details}";

            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);
            log.Debug(msg);

            if (IsChildTestNull) return null;

            if (pathImg != null)
                _childTest = _childTest.Warning(details, MediaEntityBuilder.CreateScreenCaptureFromPath(pathImg).Build());
            else
                _childTest = _childTest.Warning(details);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogInformation(string details = "LOG INFO", string? pathImg = null)
        {
            if (details != string.Empty)
            {
                System.Diagnostics.Debug.WriteLine($"{details}", UtilsHelper.log_prefix);
                log.Debug($"{details}");
            }

            if (IsChildTestNull) return null;

            if (pathImg is null)
                _childTest = _childTest.Info(details);
            else
                _childTest = _childTest.Info(details, MediaEntityBuilder.CreateScreenCaptureFromPath(pathImg).Build());

            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogPass( string details = "LOG", string? pathImg = null)
        {
            string msg = $"    *** PASSED ***  ==  {details}";

            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);

            log.Debug(msg);

            if (IsChildTestNull) return null;

            if (pathImg is null)
                _childTest = _childTest.Pass(details);
            else
                _childTest = _childTest.Pass(details, MediaEntityBuilder.CreateScreenCaptureFromPath(pathImg).Build());
            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        #endregion

        #region "Log report and capture WHOLE screen"

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogFailAndCaptureFullScreen(IWebDriver driver, string details)
        {
            string msg = $"    *** FAILURE ***  ==  {details}";
            System.Diagnostics.Debug.WriteLine(msg, UtilsHelper.log_prefix);
            log.Debug(msg);
            if (IsChildTestNull) return null;

            try
            {
                _childTest = _childTest.Fail(
                details,
                MediaEntityBuilder.CreateScreenCaptureFromPath(UtilsHelper.CaptureScreen(driver)).Build()
                );
            }
            catch (Exception ex)
            {
                string ex_msg = $"    *** ERROR ***  ==  {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(ex_msg, UtilsHelper.log_prefix);
                log.Debug(ex_msg);

                _childTest = _childTest.Fail(details);
            }

            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogWarningAndCaptureFullScreen(IWebDriver driver, string details)
        {
            string msg = $"    *** WARNING ***  ==  {details}";
            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);
            log.Debug(msg);
            if (IsChildTestNull) return null;

            try
            {
                _childTest = _childTest.Warning(
                details,
                MediaEntityBuilder.CreateScreenCaptureFromPath(UtilsHelper.CaptureScreen(driver)).Build()
                );
            }
            catch (Exception ex)
            {
                string ex_msg = $"    *** ERROR ***  ==  {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine($"{ex_msg}", UtilsHelper.log_prefix);
                log.Debug(ex_msg);

                _childTest = _childTest.Warning(details);
            }

            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogInfoAndCaptureFullScreen(IWebDriver driver, string details)
        {
            System.Diagnostics.Debug.WriteLine($"{details}", UtilsHelper.log_prefix);
            log.Debug($"{details}");
            if (IsChildTestNull) return null;

            try
            {
                string img_path = UtilsHelper.CaptureScreen(driver);
                MediaEntityBuilder img = MediaEntityBuilder.CreateScreenCaptureFromPath(img_path);
                _childTest = _childTest.Info(details, img.Build());
            }
            catch (Exception ex)
            {
                string ex_msg = $"    *** ERROR ***  ==  {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine($"{ex_msg}", UtilsHelper.log_prefix);
                log.Debug(ex_msg);

                _childTest = _childTest.Info(details);
            }

            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ExtentTest LogPassAndCaptureFullScreen(IWebDriver driver, string details)
        {
            string msg = $"    *** PASSED ***  ==  {details}";
            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);
            log.Debug(msg);
            if (IsChildTestNull) return null;

            try
            {
                _childTest = _childTest.Pass(
                details,
                MediaEntityBuilder.CreateScreenCaptureFromPath(UtilsHelper.CaptureScreen(driver)).Build()
                );
            }
            catch (Exception ex)
            {
                string ex_msg = $"    *** ERROR ***  ==  {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(ex_msg, UtilsHelper.log_prefix);
                log.Debug(ex_msg);

                _childTest = _childTest.Pass(details);
            }

            System.Threading.Thread.Sleep(100);
            return _childTest;
        }

        #endregion

        #region "Screenshot"

        /// <summary>
        /// Take screenshot and log
        /// </summary>
        /// <param name="info"></param>
        /// <param name="img"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LogScreenshot(string info, string img)
        {
            string msg = $"    *** LOG SCREEN ***    ";
            System.Diagnostics.Debug.WriteLine($"{msg}", UtilsHelper.log_prefix);
            log.Debug(msg);
            if (IsChildTestNull) return;

            _childTest.Info(info, MediaEntityBuilder.CreateScreenCaptureFromBase64String(img).Build());
        }

        #endregion

        #region "Verify Child Test"
        /// <summary>
        /// Verify test case in a section is null or not
        /// </summary>
        public static bool IsChildTestNull
        {
            get
            {
                if (_childTest is null)
                {
                    log.Debug($"Verify child test is null or not. Child test is null");
                    return true;
                }
                else
                {
                    log.Debug($"Verify child test is null or not. Child test is NOT null");
                    return false;
                }
            }
        }
        #endregion

    }
}
