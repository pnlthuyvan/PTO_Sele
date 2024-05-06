using Newtonsoft.Json;
using PTO.Base;
using PTO.Models.RestSharp;
using PTO.Utilities;
using RestSharp;

namespace PTO.API.RestSharp
{
    public static class APIs
    {
        private static readonly string apiUrl = BaseValues.APIUrl;
        private static readonly RestClient client = CreateRestClient();

        private static RestClient CreateRestClient()
        {
            var client = new RestClient(apiUrl);
            client.AddDefaultHeader("Authorization", $"Bearer {BaseValues.ApiToken}");
            return client;
        }

        public static RestResponse GetRequest()
        {
            var request = new RestRequest();
            RestResponse response = client.Execute(request);
            return response;
        }

        private static RestResponse ExecuteRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            RestResponse response = client.Execute(request);
            return response;
        }

        #region Job APIs
        /// <summary>
        /// Get Job by Job Number
        /// </summary>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        public static RestResponse GetJobByNumber(string jobNumber)
        {
            string resource = $"/Jobs/Jobs/Number/{jobNumber}";
            return ExecuteRequest(resource, Method.Get);
        }

        /// <summary>
        /// Get Job Id by Job Number
        /// </summary>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        public static int GetJobId(string jobNumber)
        {
            string resource = $"/Jobs/Jobs/Number/{jobNumber}";
            var response = ExecuteRequest(resource, Method.Get);

            if (response.IsSuccessful)
            {
                var jobData = JsonConvert.DeserializeObject<JobData_RestSharp>(response.Content);

                if (jobData.Id > 0)
                {
                    return jobData.Id;
                }
            }
            return -1;
        }

        /// <summary>
        /// Create new job via API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RestResponse CreateNewJob(JobData_RestSharp data)
        {
            string resource = $"/Jobs/Jobs";
            var request = new RestRequest(resource, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var jsonObject = new
            {
                data.Number,
                data.Jobs_SalesRep,
                data.Jobs_EstimatedValue,
                data.StatusCategories_Id,
                data.Jobs_Notes
            };
            request.AddJsonBody(jsonObject);
            RestResponse responseCreate = client.Execute(request);
            return responseCreate;
        }

        /// <summary>
        /// Update Job via API
        /// </summary>
        /// <param name="updateData"></param>
        /// <returns></returns>
        public static RestResponse UpdateJob(int jobId, JobData_RestSharp updateData)
        {
            // Update job
            string resource = $"/Jobs/Jobs/{jobId}";
            var request = new RestRequest(resource, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            var jsonObject = new
            {
                id = jobId,
                updateData.Number,
                updateData.Jobs_SalesRep,
                updateData.Jobs_EstimatedValue,
                updateData.StatusCategories_Id,
                updateData.Jobs_Notes
            };
            request.AddJsonBody(jsonObject);
            RestResponse responseUpdate = client.Execute(request);
            return responseUpdate;
        }

        /// <summary>
        /// Delete job via API
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static RestResponse DeleteJobById(int jobId)
        {
            string resource = $"/Jobs/Jobs/{jobId}";
            var request = new RestRequest(resource, Method.Delete);
            RestResponse responseUpdate = client.Execute(request);
            return responseUpdate;
        }
        #endregion
    }

}
