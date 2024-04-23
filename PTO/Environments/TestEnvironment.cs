using log4net;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Constants;
using PTO.Utilities;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;

namespace PTO.Manager
{
    public abstract class TestEnvironment
    {
        public static ILog Log;
        public static bool active_status = false;
        private static readonly ExtentReports extent = BaseValues.GetExtentReports();

        /// <summary>
        /// Get All cookies (token) to sign in
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static IList<Cookie> GetCookies(IWebDriver driver)
        {
            // Example: Extracting token from cookies
            return driver.Manage().Cookies.AllCookies;
        }

        /// <summary>
        /// Apply default setting from json config file
        /// </summary>
        public static void ApplyDefaultSettings()
        {
            string directoryPath = BaseValues.WorkingDir;

            // Check if the directory exists
            if (Directory.Exists(directoryPath))
            {
                // Delete the directory and its contents
                Directory.Delete(directoryPath, true);
                Console.WriteLine("Deleted existing directory.");
            }

            // Create report folder
            Directory.CreateDirectory(directoryPath);

            GlobalContext.Properties["LogFileName"] = $@"{BaseValues.WorkingDir}\\myLog.log";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log4net.config"));
            Log = LogManager.GetLogger(typeof(TestEnvironment));

            UtilsHelper.DebugOutput("Loading report configuration...", false);

            try
            {
                var htmlReporter = new ExtentSparkReporter(BaseValues.WorkingDir + "\\spark.html");
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var trimmedPath = !string.IsNullOrEmpty(assemblyPath) ? assemblyPath[..^16] : string.Empty;

                htmlReporter.LoadJSONConfig(trimmedPath + "Environments\\spark-config.json");
                extent.AttachReporter(htmlReporter);
            }
            catch (Exception e)
            {
                UtilsHelper.test_output($"Failed to load json config file for extent report. Exception: {e}");
            }

            UtilsHelper.InitLog();
            UtilsHelper.DebugOutput("Finished preparing Report Service~", false);
        }

        /// <summary>
        /// Write down the report and dispose the driver
        /// </summary>
        protected static void WriteReportAndDisposeDriver(IWebDriver driver)
        {
            if (driver == null) return;

            try
            {
                // Flush report (init html report file)
                extent.Flush();

                // Dispose driver instance
                driver.Quit();
            }
            catch (TypeLoadException ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi TypeLoadException: {ex.Message}");
            }

            finally
            {
                System.Threading.Thread.Sleep(250);
                GC.Collect();
                if (driver != null)
                {
                    BaseValues.FirstDriverTest = null;
                }
                CleanUpSession();
            }
        }

        /************************************************ DEV OPS methods ************************************************/

        /// <summary>
        /// Clear driver after finishing all test scripts
        /// </summary>
        public static void CleanUpSession()
        {
            Log?.Debug("Cleaning up session - removing web drivers from system processes...");
            if (string.IsNullOrEmpty(BaseValues.Browser))
                return;

            try
            {
                if (BaseValues.Browser.Equals(Browser.Chrome.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    KillProcessAndChildren("chromedriver.exe");
                }
                else if (BaseValues.Browser.Equals(Browser.Firefox.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    KillProcessAndChildren("geckodriver.exe");
                }
                else if (BaseValues.Browser.Equals(Browser.IE.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // For IE
                    KillProcessAndChildren("IEDriverServer.exe");
                }
            }
            catch { }
        }

        [DebuggerNonUserCode]
        public static void KillProcessAndChildren(string p_name)
        {
            // ManagementObjectSearcher only supports window
            if (!OperatingSystem.IsWindows())
                return;

            var assemblyDirector = AssemblyDirectory();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
            ("Select * From Win32_Process Where Name = '" + p_name + "'");
            var moc = searcher.Get().Cast<ManagementObject>().ToList();
            var currentDriverProcess = moc.Where(mo => mo["ExecutablePath"].ToString().Contains(assemblyDirector)).FirstOrDefault();
            if (null != currentDriverProcess) KillProcessAndChildren(Convert.ToInt32(currentDriverProcess["ProcessID"]));
        }

        [DebuggerNonUserCode]
        public static void KillProcessAndChildren(int pid)
        {
            // ManagementObjectSearcher only supports window
            if (!OperatingSystem.IsWindows())
                return;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher
         ("Select * From Win32_Process Where ParentProcessID = '" + pid + "'");
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {

                try
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                catch (Exception)
                {
                    // Caught system level exception during clean-up (Process no longer exists or is inaccessible)
                }
            }

            if (moc.Count > 0)
            {
                try
                {
                    Process proc = Process.GetProcessById(pid);

                    proc.Kill();
                }
                catch (Exception)
                {
                    // Caught system level exception during clean-up (Process no longer exists or is inaccessible)
                }
                finally
                {
                    searcher.Dispose();
                }
            }
        }

        private static string AssemblyDirectory()
        {
            string codeBase = Assembly.GetEntryAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

    }
}
