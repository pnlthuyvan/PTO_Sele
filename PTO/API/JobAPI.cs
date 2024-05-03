using Newtonsoft.Json;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Models;
using System.Net.Http.Headers;

namespace PTO.API
{
    public class JobAPI
    {
        private HttpClient httpClient = new HttpClient();
        private string URL = BaseValues.BaseURL + "/Job/GetAllLBMJobs?sort=&page=1&pageSize=10&group=&filter=";

        public async Task<string> GetAllJobs()
        {
            HttpRequestHeaders requestHeaders = httpClient.DefaultRequestHeaders;
            requestHeaders.Add("Accept", "application/json, text/javascript, */*;q=0.01");
            requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            string cookiesString = ConvertCookiesToString(BaseValues.LoginCookieList);
            requestHeaders.Add("Cookie", cookiesString);
            // Make api request call to get jobs
            var response = await httpClient.GetAsync(URL);
            
            //Response data
            HttpContent responseContent = response.Content;
            Task<string> reponseData = responseContent.ReadAsStringAsync();

            //Close connection - another way: using block to create and auto dispose HttpClient
            httpClient.Dispose();

            return reponseData.Result;
        }

        public static string ConvertCookiesToString(IList<Cookie> cookies)
        {
            if (cookies == null || !cookies.Any())
                return string.Empty;

            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }

        public JobData[] ConvertJsonToJob(string json)
        {
            var wrapper = JsonConvert.DeserializeObject<JobDataWrapper>(json);
            // Access the JobData objects from the wrapper
            return wrapper.Data;
        }

    }

}
