using PTO.Base;
using RestSharp;

namespace PTO.API.RestSharp
{
    public class Token
    {
        private const string YourUsername = "4c0c7524-2ecd-4d5e-ad97-06dfccbea071";
        private const string YourPassword = "EgvvY04u9Uag/Arj5Ue8KQ==";

        /// <summary>
        /// Get access token for API
        /// </summary>
        public void SetToken()
        {
            var client = new RestClient("https://293-app-beta-carbonite-sso.dv3.bplhost.com");
            var request = new RestRequest("/common/oauth2/v1.0/token", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                $"grant_type=client_credentials&client_id={YourUsername}&client_secret={YourPassword}&scope=openid WebAPI.All WebAPI.LBM&aud=web_api", ParameterType.RequestBody);

            RestResponse<Models.RestSharp.Token> response = client.Execute<Models.RestSharp.Token>(request);

            if (response.IsSuccessful)
            {
                Models.RestSharp.Token access_token = response.Data;
                BaseValues.ApiToken = access_token.access_token;

                Console.WriteLine($"Token: {BaseValues.ApiToken}");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }
}
