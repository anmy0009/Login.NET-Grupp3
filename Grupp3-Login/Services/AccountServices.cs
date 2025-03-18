using Microsoft.AspNetCore.Identity.Data;

namespace Grupp3_Login.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AccountService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiBaseUrl"];
        }

        public async Task<bool> RegisterCustomerAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/account/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
