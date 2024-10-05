using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using OnlineBookstore.main.config;

namespace OnlineBookstore.main.requests
{
    public class BaseRequests
    {
        private readonly RestClient _client;
        public readonly IConfiguration _config;

        // Load configuration in the constructor by default using ConfigBuilder
        public BaseRequests()
        {
            _config = ConfigBuilder.LoadConfiguration();
            _client = CreateRestClient();
        }

        private RestClient CreateRestClient()
        {
            var baseUrl = _config["BaseUrl"];
            return new RestClient(new RestClientOptions { BaseUrl = new Uri(baseUrl) });
        }

        public RestResponse ExecuteRequest(string endpoint, Method method, object body = null!)
        {
            var request = CreateRequest(endpoint, method);
            if (body != null!)
            {
                request.AddJsonBody(body);
            }

            return method switch
            {
                Method.Post => _client.Post(request),
                Method.Put => _client.Put(request),
                Method.Delete => _client.Delete(request),
                _ => _client.Get(request)
            };
        }
        
        private static RestRequest CreateRequest(string endpoint, Method method = Method.Get)
        {
            return new RestRequest(endpoint, method);
        }

        public static void VerifyStatusCode(RestResponse response, HttpStatusCode expectedStatusCode, string errorMessage)
        {
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), errorMessage);
        }

        public static T DeserializeResponse<T>(RestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
