using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using PTO.Utilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PTO.Base
{
    [TestFixture]

    public abstract class BaseTestScript
    {
        public required string sectionName;
        public string testName;
        public string testCode;

        #region "Set up Section report"

        /// <summary>
        /// Set up the Test Set name
        /// </summary>
        public abstract void SetupTestSectionName();

        /// <summary>
        /// Set the test set name
        /// </summary>
        /// <param name="parentname"></param>
        protected void SetupTestSectionName(string parentname)
        {
            // Executes once for the test class.
            if (string.IsNullOrEmpty(parentname))
            {
                string sec_name = string.Join("", Regex.Split(GetType().Namespace, @"([^\w\.])").Select(ns =>
                        ns.Substring(ns.LastIndexOf('.') + 1))
                    );

                parentname = sec_name;
            }
            this.AssignTestSection(parentname);
            ExtentReportsHelper.SwitchTestSet(parentname);

        }

        private void AssignTestSection(string test_section)
        {
            this.sectionName = test_section;
        }

        #endregion

        #region "Set up Test case report"

        /// <summary>
        /// Override the test case name
        /// </summary>
        public virtual void SetupTestCaseName()
        {
            CreateOrUpdateTestCaseName(string.Empty);
        }

        protected virtual void CreateOrUpdateTestCaseName(string testName, string description = "")
        {
            // Executes once for the test class.
            if (string.IsNullOrEmpty(testName))
                ExtentReportsHelper.CreateTest(TestContext.CurrentContext.Test.Name, description);
            else
                ExtentReportsHelper.CreateOrUpdateTest(this.testName, testName, description);

            this.testCode = GetType().Name;
            this.testName = TestContext.CurrentContext.Test.Name;
        }

        #endregion

        #region "Set Up / Tear Down"

        [OneTimeSetUp]
        public virtual void OnSectionStart()
        {
            SetupTestSectionName();
        }

        [SetUp]
        public virtual void PreConditionSetUpTest()
        {
            SetupTestCaseName();

            UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);
            UtilsHelper.DebugOutput($"    ================== Testing @ {testCode} ==================");
            UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);
        }

        [TearDown]
        public virtual void TearDown()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Failed && ExtentReportsHelper.IsChildTestNull)
            {
                if (ExtentReportsHelper.IsParentTestNull)
                    SetupTestSectionName();
                SetupTestCaseName();
            }
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.Message)
                ? ""
                : $"<pre>{TestContext.CurrentContext.Result.Message}</pre>";
            var logstatus = status switch
            {
                TestStatus.Failed => Status.Fail,
                TestStatus.Inconclusive => Status.Warning,
                TestStatus.Skipped => Status.Skip,
                _ => Status.Pass,
            };
            ExtentReportsHelper.GetTest().Log(logstatus, "Test result finished with status " + logstatus + stacktrace);

            try
            {
                if (logstatus == Status.Fail)
                    ExtentReportsHelper.LogFail($"Test '{this.testCode}.{this.testName}' in Section '{this.sectionName}' has failed.");
            }
            catch (WebDriverException ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
            UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);
            UtilsHelper.DebugOutput($"    ================== Completed # {this.testName} ==================");
            UtilsHelper.DebugOutput("-----------------------------------------------------------------------------------------", false);
        }

        #endregion
    }
}
