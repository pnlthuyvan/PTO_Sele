using AventStack.ExtentReports;
using OpenQA.Selenium;
using PTO.Constants;
using PTO.Utilities;

namespace PTO.Base
{
    public static class BaseValues
    {
        private static readonly Lazy<ExtentReports> _lazyExtent = new Lazy<ExtentReports>(() => new ExtentReports());

        public static ExtentReports GetExtentReports()
        {
            return _lazyExtent.Value;
        }

        public static IList<Cookie>? LoginCookieList { get; set; }

        public static IWebDriver? FirstDriverTest { get; set; }
        public static IWebDriver? driver_section1 { get; set; }

        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? AppName => AppConfigUtil.GetAppSetting(BaseConstants.AppName);

        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? Version => AppConfigUtil.GetAppSetting(BaseConstants.Version);


        /// <summary>
        /// Read protocol from appsettings.json file
        /// </summary>
        public static string? Protocol => AppConfigUtil.GetAppSetting(BaseConstants.ApplicationProtocol);

        /// <summary>
        /// Read Tenant from appsettings.json file
        /// </summary>
        public static string? Tenant => AppConfigUtil.GetAppSetting(BaseConstants.Tenant);

        /// <summary>
        /// Read Domain from appsettings.json file
        /// </summary>
        public static string? ApplicationDomain => AppConfigUtil.GetAppSetting(BaseConstants.ApplicationDomain);

        /// <summary>
        /// Read UserName from appsettings.json file
        /// </summary>
        public static string? UserName => AppConfigUtil.GetAppSetting(BaseConstants.UserName);

        /// <summary>
        /// Read Password from appsettings.json file
        /// </summary>
        public static string? Password => AppConfigUtil.GetAppSetting(BaseConstants.Password);

        /// <summary>
        /// Read Report Location from appsettings.json file
        /// </summary>
        public static string? ReportLocation => AppConfigUtil.GetAppSetting(BaseConstants.ReportLocation);

        /// <summary>
        /// Read Report counter from appsettings.json file
        /// </summary>
        public static string? ReportCounter => AppConfigUtil.GetAppSetting(BaseConstants.ReportCounter);

        /// <summary>
        /// Read browser name from appsettings.json file
        /// </summary>
        public static string? Browser => AppConfigUtil.GetAppSetting(BaseConstants.Browser);

        /// <summary>
        /// Read browser headless mode from appsettings.json file
        /// </summary>
        public static bool Headless => !string.IsNullOrEmpty(AppConfigUtil.GetAppSetting(BaseConstants.Headless)) && bool.Parse(AppConfigUtil.GetAppSetting(BaseConstants.Headless));

        /// <summary>
        /// Read debug mode status from appsettings.json file
        /// </summary>
        public static bool DebugMode => !string.IsNullOrEmpty(AppConfigUtil.GetAppSetting(BaseConstants.DebugMode)) && bool.Parse(AppConfigUtil.GetAppSetting(BaseConstants.DebugMode));

        /// <summary>
        /// Read debug port from appsettings.json file
        /// </summary>
        public static int DebugPort => int.Parse(AppConfigUtil.GetAppSetting(BaseConstants.DebugPort));

        public static int PageLoadTimeOut => int.Parse(AppConfigUtil.GetAppSetting(BaseConstants.PageLoadTimeOut));


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
    }
}
