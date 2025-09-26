using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PpeBackendAPI.Tests
{
    public class PerfilControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PerfilControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> ObterTokenAsync()
        {
            var payload = new
            {
                Email = "admin@gmail.com",
                Senha = "216125"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(body);
            return json.RootElement.GetProperty("token").GetString();
        }

        [Fact]
        public async Task Perfil_ComAutenticacao_DeveRetornar200()
        {
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/perfil");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("email", body); // ou qualquer campo esperado no perfil
        }
    }
}
