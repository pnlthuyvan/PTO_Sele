using Newtonsoft.Json;
using PTO.Base;
using PTO.Models;
using RestSharp;
using System.Linq;

namespace PTO.API
{
    public class Bearer
    {
        string apiUrl = "https://293-app-dev-pipeline-api.azurewebsites.net";
        public static string token; // Replace with your token
        string your_username = "4c0c7524-2ecd-4d5e-ad97-06dfccbea071";
        string your_password = "EgvvY04u9Uag/Arj5Ue8KQ==";

        //write method to use GET api with restsharp
        public RestResponse GetRequest()
        {
            var client = new RestClient(apiUrl);
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {token}");

            RestResponse response = client.Execute(request);
            return response;
        }

        //write method to through authentication and return token 
        public void SetToken()
        {
            var client = new RestClient("https://293-app-beta-carbonite-sso.dv3.bplhost.com");
            var request = new RestRequest("/common/oauth2/v1.0/token", Method.Post);


            // Set the content type of the request body
            // Set content type and accept headers to indicate JSON format
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", 
                $"grant_type=client_credentials&client_id=4c0c7524-2ecd-4d5e-ad97-06dfccbea071&client_secret=EgvvY04u9Uag/Arj5Ue8KQ=='&scope=openid WebAPI.All WebAPI.LBM& aud=web_api", ParameterType.RequestBody);
            //request.AddParameter("Content-Type", "application/x-www-form-urlencoded");
            //request.AddParameter("Accept", "application/json");

            //request.AddJsonBody(new { username = $"{your_username}", password = $"{your_password}",
            //    grant_type = "client_credentials",
            //    scope = "openid WebAPI.All WebAPI.LBM",
            //    aud = "web_api"
            //});

            RestResponse response = client.Execute(request);

            // Check if the response is successful
            if (response.IsSuccessful)
            {
                // Retrieve the token from the response headers
                //string token = response.Headers.("Authorization").FirstOrDefault();

                string content = response.Content.ToString();
                var access_token = JsonConvert.DeserializeObject<Token>(content);
                token = access_token.access_token;

                Console.WriteLine($"Token: {token}");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

}
