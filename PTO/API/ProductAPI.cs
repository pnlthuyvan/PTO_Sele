using Newtonsoft.Json;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Models;
using System.Net.Http.Headers;

namespace PTO.API
{
    public class ProductAPI
    {
        private string URL = BaseValues.BaseURL + "/Takeoff/UpdateProductBySection";

        public async Task<string> UpdateProductBySection()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestHeaders requestHeaders = client.DefaultRequestHeaders;
                requestHeaders.Add("Accept", "application/json, text/javascript, */*;q=0.01");
                requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                string cookiesString = ConvertCookiesToString(BaseValues.LoginCookieList);
                requestHeaders.Add("Cookie", cookiesString);

                //Rest API
                UpdateProductDetail updateProductDetailData = new UpdateProductDetail
                {
                    JobId = Guid.Parse("3c2905d1-3bfb-4198-a02e-bca37c71ec71"),
                    ProductId = Guid.Parse("6a3ee173-0c73-4d2c-b536-9b6f12333e88"),
                    EstimatingSectionId = 176,
                    EstimatingUseId = null, // This was empty in your description
                    NewEstimatingSectionId = 176,
                    NewEstimatingSectionName = "Section_PLBM_7434_2",
                    NewEstimatingUseId = 78,
                    NewEstimatingUseName = "QA_Use_PLBM_4868",
                    NewColorCode = "#5400f0"
                };
                //string data = JsonConvert.SerializeObject(updateProductDetailData);
                //var contentData = new StringContent(data);

                //FormUrlEncodedContent to stimulate an ajax call form data
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"JobId", updateProductDetailData.JobId.ToString()},
                    {"ProductId", updateProductDetailData.ProductId.ToString()},
                    {"EstimatingSectionId", updateProductDetailData.EstimatingSectionId.ToString()},
                    {"EstimatingUseId", updateProductDetailData.EstimatingUseId.HasValue ? updateProductDetailData.EstimatingUseId.Value.ToString() : ""},
                    {"NewEstimatingSectionId", updateProductDetailData.NewEstimatingSectionId.ToString()},
                    {"NewEstimatingSectionName", updateProductDetailData.NewEstimatingSectionName},
                    {"NewEstimatingUseId", updateProductDetailData.NewEstimatingUseId.ToString()},
                    {"NewEstimatingUseName", updateProductDetailData.NewEstimatingUseName},
                    {"NewColorCode", updateProductDetailData.NewColorCode}
                });

                // Make api request call to get jobs
                var response = await client.PostAsync(URL, content);

                //Response data
                HttpContent responseContent = response.Content;
                Task<string> reponseData = responseContent.ReadAsStringAsync();

                return reponseData.Result;
            }

        }

        public static string ConvertCookiesToString(IList<Cookie> cookies)
        {
            if (cookies == null || !cookies.Any())
                return string.Empty;

            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }

    }
}
