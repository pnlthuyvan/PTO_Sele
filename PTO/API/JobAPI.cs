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


        public async Task<string> GetAllJobs_Thanh()
        {
            HttpRequestHeaders requestHeaders = httpClient.DefaultRequestHeaders;
            requestHeaders.Add("Accept", "application/json, text/javascript, */*;q=0.01");
            //requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            //string cookiesString = ConvertCookiesToString(BaseValues.LoginCookieList);
            //requestHeaders.Add("Cookie", cookiesString);
            // Make api request call to get jobs
            requestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromSeconds(20);

            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjNFMEQ0OUFCQjg4ODQyQTk4NkI4NkQ5RjFFQzNBNThDMTRBMENGNDAiLCJ4NXQiOiJQZzFKcTdpSVFxbUd1RzJmSHNPbGpCU2d6MEEiLCJ0eXAiOiJhdCtqd3QifQ.eyJzdWIiOiI0YzBjNzUyNC0yZWNkLTRkNWUtYWQ5Ny0wNmRmY2NiZWEwNzEiLCJuYW1lIjoiNGMwYzc1MjQtMmVjZC00ZDVlLWFkOTctMDZkZmNjYmVhMDcxIiwib2lfcHJzdCI6IjRjMGM3NTI0LTJlY2QtNGQ1ZS1hZDk3LTA2ZGZjY2JlYTA3MSIsImNsaWVudF9pZCI6IjRjMGM3NTI0LTJlY2QtNGQ1ZS1hZDk3LTA2ZGZjY2JlYTA3MSIsIm9pX3Rrbl9pZCI6Ijk2NWNkZDc3LTA5YzMtNDZkMi04MGM3LTNiNGQ4ZjVjMWY0NyIsImF1ZCI6IndlYl9hcGkiLCJzY29wZSI6Im9wZW5pZCBXZWJBUEkuQWxsIFdlYkFQSS5MQk0iLCJqdGkiOiIyOWY3YWVjYy1mN2RlLTRmOWEtODNlYy0yMTRkMmQwZTRmOTgiLCJpc3MiOiJodHRwczovLzI5My1hcHAtYmV0YS1jYXJib25pdGUtc3NvLmR2My5icGxob3N0LmNvbS8iLCJleHAiOjE3MTU0MjUwNjMsImlhdCI6MTcxNDgyMDI2M30.vNP5_NaHKrKCEvfGyejL54En67unrvJzVzf_dN8iGr4Waf5wB3lyBJY40FliXpLYjWKB0lsFG40T0T05XGo0CAGjoRezo9BnzKC_0sCX3dME5StkJoGhuhAHl24rM2R2GGDQkIBIIT-bY3RQ9Fsa7N2_X2wy7wHRzSdEZx-Sq5hLzG6j0skWdZdHovLtLM8R8EjSCfrsyGPFf4lvHJNMcuqWSAJibGjcDieu-6kF6PSGX5CDWXq9Ne4xBgoykbMTCEKR8hz7ZrX1enP4D0ESj-bzq82ZK1MQSKRiABYgKE5potAX4xNHcnsiF5wAV8foZxFLTD3GoAtOzridAmGUhg";
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var response = await httpClient.GetAsync(URL);

            //Response data
            HttpContent responseContent = response.Content;
            Task<string> reponseData = responseContent.ReadAsStringAsync();

            //Close connection - another way: using block to create and auto dispose HttpClient
            httpClient.Dispose();

            return reponseData.Result;
        }





        public async Task<string> GetAllJobs_Van()
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
