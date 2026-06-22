using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTOs;
using TCC.Contabilidade.API.Controllers;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class AuthIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_And_Login_ShouldSucceed()
    {
        // 1. Register
        var registerRequest = new AuthController.RegisterRequest(
            "Test Admin",
            "admin@test.com",
            "StrongPassword123!",
            "Admin"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var registerData = await registerResponse.Content.ReadFromJsonAsync<ApiResponseDTO<UserRegistrationResponseDTO>>();
        Assert.True(registerData?.Sucesso);
        Assert.Equal("admin@test.com", registerData.Dados.Email);

        // 2. Login
        var loginRequest = new AuthController.LoginRequest(
            "admin@test.com",
            "StrongPassword123!"
        );

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        Assert.True(loginData?.Sucesso);
        Assert.NotNull(loginData.Dados.AccessToken);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldFail()
    {
        var loginRequest = new AuthController.LoginRequest(
            "admin@test.com",
            "WrongPassword"
        );

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}
