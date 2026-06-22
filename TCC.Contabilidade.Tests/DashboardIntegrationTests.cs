using System.Net.Http.Headers;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.API.Controllers;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class DashboardIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public DashboardIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboardSummary_ShouldSucceed()
    {
        // 1. Authenticate as Contador
        var email = $"contador_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new AuthController.RegisterRequest("Test Contador", email, "Pass123!", "Contador"));
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new AuthController.LoginRequest(email, "Pass123!"));
        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Dados.AccessToken);

        // 2. Get Dashboard Summary
        var response = await _client.GetAsync("/api/Dashboard/resumo");
        response.EnsureSuccessStatusCode();

        var dashboardData = await response.Content.ReadFromJsonAsync<ApiResponseDTO<DashboardSummaryDTO>>();
        Assert.True(dashboardData?.Sucesso);
        Assert.NotNull(dashboardData.Dados);
    }
}
