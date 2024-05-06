using Newtonsoft.Json;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Models.Microsoft;
using System.Net.Http.Headers;

namespace PTO.API.Microsoft
{
    public class JobAPI
    {
        private HttpClient httpClient = new HttpClient();
        private readonly string URL = BaseValues.BaseURL + "/Job/GetAllLBMJobs?sort=&page=1&pageSize=10&group=&filter=";

        public async Task<string> GetAllJobs()
        {
            HttpRequestHeaders requestHeaders = httpClient.DefaultRequestHeaders;
            requestHeaders.Add("Accept", "application/json, text/javascript, */*;q=0.01");
            requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            string cookiesString = ConvertCookiesToString(BaseValues.LoginCookieList);
            requestHeaders.Add("Cookie", cookiesString);

            using (var response = await httpClient.GetAsync(URL))
            {
                using (HttpContent responseContent = response.Content)
                {
                    return await responseContent.ReadAsStringAsync();
                }
            }
        }

        public static string ConvertCookiesToString(IList<Cookie> cookies)
        {
            if (cookies == null || cookies.Count == 0)
                return string.Empty;

            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }

        public JobData[] ConvertJsonToJob(string json)
        {
            var wrapper = JsonConvert.DeserializeObject<JobDataWrapper>(json);
            return wrapper.Data;
        }
    }

}
