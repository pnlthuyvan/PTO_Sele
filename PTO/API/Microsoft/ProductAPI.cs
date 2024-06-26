﻿using Newtonsoft.Json;
using OpenQA.Selenium;
using PTO.Base;
using PTO.Models.Microsoft;
using System.Net.Http.Headers;

namespace PTO.API.Microsoft
{
    public class ProductAPI
    {
        private readonly string URL = BaseValues.BaseURL + "/Takeoff/UpdateProductBySection";

        public async Task<string> UpdateProductBySection()
        {
            using (HttpClient client = new())
            {
                HttpRequestHeaders requestHeaders = client.DefaultRequestHeaders;
                requestHeaders.Add("Accept", "application/json, text/javascript, */*;q=0.9");
                requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                string cookiesString = ConvertCookiesToString(BaseValues.LoginCookieList);
                requestHeaders.Add("Cookie", cookiesString);

                //Rest API
                UpdateProductDetail updateProductDetailData1 = new UpdateProductDetail
                {
                    JobId = "0b27f84b-7b28-472e-a2b5-c8d944f23599",
                    ProductId = "1e2575fc-92fc-4eac-809f-2bfa39e5414f",
                    EstimatingSectionId = 30,
                    EstimatingUseId = 403, // This was empty in your description
                    NewEstimatingSectionId = 30,
                    NewEstimatingSectionName = "QA_RT_Section_Auto_PLBM_6848",
                    NewEstimatingUseId = 420,
                    NewEstimatingUseName = "QA_Use_PLBM_6847",
                    NewColorCode = "#c60c0c"
                };

                //FormUrlEncodedContent to stimulate an ajax call form data
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"JobId", updateProductDetailData1.JobId.ToString()},
                    {"ProductId", updateProductDetailData1.ProductId.ToString()},
                    {"EstimatingSectionId", updateProductDetailData1.EstimatingSectionId.ToString()},
                    {"EstimatingUseId", updateProductDetailData1.EstimatingUseId.HasValue ? updateProductDetailData1.EstimatingUseId.Value.ToString() : ""},
                    {"NewEstimatingSectionId", updateProductDetailData1.NewEstimatingSectionId.ToString()},
                    {"NewEstimatingSectionName", updateProductDetailData1.NewEstimatingSectionName},
                    {"NewEstimatingUseId", updateProductDetailData1.NewEstimatingUseId.ToString()},
                    {"NewEstimatingUseName", updateProductDetailData1.NewEstimatingUseName},
                    {"NewColorCode", updateProductDetailData1.NewColorCode}
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
