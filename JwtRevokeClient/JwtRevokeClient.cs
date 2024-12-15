using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace JwtRevoke
{
    public class JwtRevokeOptions
    {
        public int MaxRetries { get; set; } = 3;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan RateLimitRetryDelay { get; set; } = TimeSpan.FromSeconds(1);
        public string BaseUrl { get; set; } = "https://api.jwtrevoke.com";
    }

    public class JwtRevokeException : Exception
    {
        public int StatusCode { get; }
        public object ResponseData { get; }

        public JwtRevokeException(string message, int statusCode, object responseData = null)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }
    }

    public class JwtRevokeClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public JwtRevokeClient(string apiKey, ServiceCollection services, JwtRevokeOptions options = null)
        {
            options ??= new JwtRevokeOptions();
            
            var handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = options.Timeout
            };
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    options.MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        private async Task<T> SendRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc)
        {
            var response = await _retryPolicy.ExecuteAsync(requestFunc);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new JwtRevokeException(
                    $"Request failed with status {response.StatusCode}",
                    (int)response.StatusCode,
                    content
                );
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent);
        }

        public async Task<RevokedTokenList> ListRevokedTokensAsync()
        {
            return await SendRequestAsync<RevokedTokenList>(async () =>
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/api/revocations/list");
                return response;
            });
        }

        public async Task<RevokeResponse> RevokeTokenAsync(string jwtId, string reason, DateTime expiryDate)
        {
            var request = new RevokeRequest
            {
                JwtId = jwtId,
                Reason = reason,
                ExpiryDate = expiryDate
            };

            return await SendRequestAsync<RevokeResponse>(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/api/revocations/revoke", request);
                return response;
            });
        }

        public async Task DeleteRevokedTokenAsync(string jwtId)
        {
            await SendRequestAsync<Task>(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/api/revocations/{jwtId}");
                return response;
            });
        }
    }
} 