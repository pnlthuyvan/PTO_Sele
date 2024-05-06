using PTO.Base;
using PTO.Utilities;
using Pipeline.Common.Constants;
using Newtonsoft.Json;
using PTO.Models.RestSharp;
using PTO.API.RestSharp;

namespace PTO.TestScripts.Demo.API_Test.RestSharp
{
    [TestFixture]
    [Parallelizable]
    public class APIs_Test : BaseTestScript
    {
        private readonly string JOB_NUMBER = "PostmanTest01";

        public override void SetupTestSectionName()
        {
            SetupTestSectionName(Sections.API);
        }

        [Test, Category($"{Sections.API}"), Order(1)]
        public void GET_APIs()
        {
            var response = APIs.GetJobByNumber(JOB_NUMBER);

            if (response.IsSuccessful)
            {
                var jobData = JsonConvert.DeserializeObject<JobData_RestSharp>(response.Content);

                if (!string.IsNullOrEmpty(jobData.Number))
                {
                    ExtentReportsHelper.LogPass($"Job '{jobData.Number}' is existing.");
                }
                else
                {
                    ExtentReportsHelper.LogInformation("Job 'Job_Vp' is NOT existing. Please create a new one.");
                }
            }
            else
            {
                ExtentReportsHelper.LogFail("Failed to call API 'GetJobByNumber'");
            }
        }

        [Test, Category($"{Sections.API}"), Order(2)]
        public void POST_APIs()
        {
            var data = new JobData_RestSharp()
            {
                Number = "PostmanTest01",
                Jobs_SalesRep = "Test",
                Jobs_EstimatedValue = "100 $",
                StatusCategories_Id = 4,
                Jobs_Notes = "Data using for PTO Testing."
            };

            int jobId = APIs.GetJobId(data.Number);
            if (jobId > 0)
            {
                APIs.DeleteJobById(jobId);
            }

            var response = APIs.CreateNewJob(data);

            if (response.IsSuccessful)
            {
                var jobData = JsonConvert.DeserializeObject<JobData_RestSharp>(response.Content);

                if (!string.IsNullOrEmpty(jobData.Number))
                {
                    ExtentReportsHelper.LogPass($"Job '{jobData.Number}' is created successfully.");
                }
                else
                {
                    ExtentReportsHelper.LogFail($"Failed to create job '{jobData.Number}'. Please check API again");
                }
            }
            else
            {
                ExtentReportsHelper.LogFail("Failed to call API 'CreateNewJob'");
            }
        }

        [Test, Category($"{Sections.API}"), Order(3)]
        public void PUT_APIs()
        {
            var updateData = new JobData_RestSharp()
            {
                Number = "PostmanTest01",
                Jobs_SalesRep = "Test_Update",
                Jobs_EstimatedValue = "200 $",
                StatusCategories_Id = 5,
                Jobs_Notes = "Data using for PTO Testing - Update."
            };

            var response = APIs.GetJobByNumber(updateData.Number);
            var jobData = JsonConvert.DeserializeObject<JobData_RestSharp>(response.Content);
            int jobId = jobData.Id;

            if (jobId <= 0)
            {
                ExtentReportsHelper.LogFail($"Job '{jobId}' does not exist. Cannot update it.");
                return;
            }

            response = APIs.UpdateJob(jobId, updateData);

            if (response.IsSuccessful)
            {
                ExtentReportsHelper.LogPass($"Job '{updateData.Number}' is updated successfully.");
            }
            else
            {
                ExtentReportsHelper.LogFail("Failed to call API 'UpdateJob'");
            }
        }

        [Test, Category($"{Sections.API}"), Order(4)]
        public void DELETE_APIs()
        {
            var response = APIs.GetJobByNumber(JOB_NUMBER);
            int jobId;

            if (response.IsSuccessful)
            {
                var jobData = JsonConvert.DeserializeObject<JobData_RestSharp>(response.Content);

                if (string.IsNullOrWhiteSpace(jobData.Number))
                {
                    ExtentReportsHelper.LogFail($"Job number '{JOB_NUMBER}' is NOT existing. Can't delete it.");
                    return;
                }
                jobId = jobData.Id;
            }
            else
            {
                ExtentReportsHelper.LogFail($"Failed to retrieve job with id '{JOB_NUMBER}'.");
                return;
            }

            response = APIs.DeleteJobById(jobId);

            if (response.IsSuccessful)
                ExtentReportsHelper.LogPass($"Job '{JOB_NUMBER}' is deleted successfully.");
            else
                ExtentReportsHelper.LogFail("Failed to call API 'DeleteJob'");
        }
    }
}
