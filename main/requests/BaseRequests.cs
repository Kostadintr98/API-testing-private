using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace OnlineBookstore.main.requests
{
    public class BaseRequests
    {
        protected readonly RestClient _client;
        protected readonly IConfiguration _config;

        public BaseRequests(IConfiguration configuration)
        {
            _config = configuration;
            _client = CreateRestClient();
        }

        private RestClient CreateRestClient()
        {
            var baseUrl = _config["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("ApiSettings:BaseUrl", "Base URL cannot be null or empty. Please check your configuration.");
            }
            return new RestClient(new RestClientOptions { BaseUrl = new Uri(baseUrl) });
        }

        private static RestRequest CreateRequest(string endpoint, Method method = Method.Get)
        {
            return new RestRequest(endpoint, method);
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