using AventStack.ExtentReports;
using OpenQA.Selenium;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Base
{
    public static class BaseValues
    {
        private static readonly Lazy<ExtentReports> _lazyExtent = new(() => new ExtentReports());

        public static ExtentReports GetExtentReports()
        {
            return _lazyExtent.Value;
        }

        public static IList<Cookie>? LoginCookieList { get; set; }

        public static IWebDriver? FirstDriverTest { get; set; }

        public static string? ApiToken { get; set; }

        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? AppName => AppConfigUtil.GetAppSetting(BaseConstants.AOO_NAME);

        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? Version => AppConfigUtil.GetAppSetting(BaseConstants.VERSION);


        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? Protocol => AppConfigUtil.GetAppSetting(BaseConstants.APPLICATION_PROTOCOL);

        /// <summary>
        /// Read Tenant from appsettings.json file
        /// </summary>
        public static string? Tenant => AppConfigUtil.GetAppSetting(BaseConstants.TENANT);

        /// <summary>
        /// Read Domain from appsettings.json file
        /// </summary>
        public static string? ApplicationDomain => AppConfigUtil.GetAppSetting(BaseConstants.APPLICATION_DOMAIN);

        /// <summary>
        /// Read UserName from appsettings.json file
        /// </summary>
        public static string? UserName => AppConfigUtil.GetAppSetting(BaseConstants.USER_NAME);

        /// <summary>
        /// Read Password from appsettings.json file
        /// </summary>
        public static string? Password => AppConfigUtil.GetAppSetting(BaseConstants.PASSWORD);

        /// <summary>
        /// Read Report Location from appsettings.json file
        /// </summary>
        public static string? ReportLocation => AppConfigUtil.GetAppSetting(BaseConstants.REPORT_LOCATION);

        /// <summary>
        /// Read Report counter from appsettings.json file
        /// </summary>
        public static string? ReportCounter => AppConfigUtil.GetAppSetting(BaseConstants.REPORT_COUNTER);

        /// <summary>
        /// Read browser name from appsettings.json file
        /// </summary>
        public static string? Browser => AppConfigUtil.GetAppSetting(BaseConstants.BROWSER);

        /// <summary>
        /// Read browser headless mode from appsettings.json file
        /// </summary>
        public static bool Headless => !string.IsNullOrEmpty(AppConfigUtil.GetAppSetting(BaseConstants.HEADLESS)) && bool.Parse(AppConfigUtil.GetAppSetting(BaseConstants.HEADLESS));

        /// <summary>
        /// Read debug mode status from appsettings.json file
        /// </summary>
        public static bool DebugMode => !string.IsNullOrEmpty(AppConfigUtil.GetAppSetting(BaseConstants.DEBUG_MODE)) && bool.Parse(AppConfigUtil.GetAppSetting(BaseConstants.DEBUG_MODE));

        /// <summary>
        /// Read debug port from appsettings.json file
        /// </summary>
        public static int DebugPort => int.Parse(AppConfigUtil.GetAppSetting(BaseConstants.DEBUG_PORT));

        /// <summary>
        /// The limit for waiting until the page loads successfully
        /// </summary>
        public static int PageLoadTimeOut => int.Parse(AppConfigUtil.GetAppSetting(BaseConstants.PAGE_LOAD_TIMEOUTS));

        /// <summary>
        /// The limit for waiting until being able to successfully retrieve an element
        /// </summary>
        public static int WaitingTimeOut => int.Parse(AppConfigUtil.GetAppSetting(BaseConstants.WAITING_TIMEOUTS));


        /// <summary>
        /// Get the project name via the tenant
        /// </summary>
        public static string? ProjectName
        {
            get
            {
                if (!string.IsNullOrEmpty(Tenant) && Tenant.StartsWith("dev01"))
                    return "PTO Automation - Dev";
                else if (!string.IsNullOrEmpty(Tenant) && Tenant.StartsWith("qa"))
                    return "PTO Automation - Beta";
                else
                    return "PTO Automation - Staging";

                //// Automation Report moved to Artifact on Teamcity, so that don't need to separate project name anymore
                //return "LBM Automation";
            }
        }

        /// <summary>
        /// Testing url that get from app config file
        /// </summary>
        public static string BaseURL
        {
            get
            {
                return BaseValues.Protocol + "://" + BaseValues.ApplicationDomain + "/" + BaseValues.Tenant;
            }
        }

        /// <summary>
        /// Here is dummy page (404 page) using for domain url set up
        /// </summary>
        public static string ErrorURL
        {
            get
            {
                return BaseValues.Protocol + "://" + BaseValues.ApplicationDomain + "/signin-oidc";
            }
        }

        public static string RelativeURL
        {
            get
            {
                return (BaseValues.FirstDriverTest != null) ? new Uri(BaseValues.FirstDriverTest.Url).AbsolutePath : "/";
            }
        }

        /// <summary>
        /// Get working dir (is buuld couter or datetime)
        /// </summary>
        public static string WorkingDir
        {
            get
            {
                string? BuildCounter = ReportCounter;
                //if (string.IsNullOrEmpty(BuildCounter))
                //    return $"{ReportLocation}\\{ProjectName}\\Report {DateTime.Now:ddd dd-MMM-yyyy HH-mm-ss}\\";
                //else
                    return $"{ReportLocation}\\{ProjectName}\\Report Build No.{BuildCounter}";
            }
        }

        /// <summary>
        /// Get APIs URL
        /// </summary>
        public static string APIUrl
        {
            get
            {
                string domain, tenant, customer;
                if (AppConfigUtil.GetAppSetting(BaseConstants.TENANT).ToString()
                    .Equals("qa", StringComparison.CurrentCultureIgnoreCase))
                {
                    domain = "https://293-app-beta-pipeline-api.dv3.bplhost.com";
                    customer = "qa";
                    tenant = "01";
                }
                else
                {
                    domain = "https://293-app-dev-pipeline-api.azurewebsites.net";
                    customer = "dev01";
                    tenant = "01";
                }
                return $"{domain}/{customer}/{tenant}"; 
            }
        }   
    }
}
