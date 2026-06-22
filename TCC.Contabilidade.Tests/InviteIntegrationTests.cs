using System.Net.Http.Headers;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.API.Controllers;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class InviteIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public InviteIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsContador()
    {
        var email = $"contador_{Guid.NewGuid()}@test.com";
        var registerRequest = new AuthController.RegisterRequest(
            "Test Contador",
            email,
            "StrongPassword123!",
            "Contador"
        );

        await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        var loginRequest = new AuthController.LoginRequest(email, "StrongPassword123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();

        return loginData.Dados.AccessToken;
    }

    [Fact]
    public async Task CreateInvite_And_RegisterWithInvite_ShouldSucceed()
    {
        // 1. Authenticate as Contador
        var token = await AuthenticateAsContador();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create Invite
        var inviteRequest = new ConvitesController.CriarConviteRequest("newclient@test.com");
        var inviteResponse = await _client.PostAsJsonAsync("/api/Convites", inviteRequest);
        inviteResponse.EnsureSuccessStatusCode();

        var inviteData = await inviteResponse.Content.ReadAsStringAsync();
        // The controller returns a dynamic object, not ApiResponseDTO
        var inviteToken = System.Text.Json.JsonDocument.Parse(inviteData).RootElement.GetProperty("token").GetString();
        Assert.NotNull(inviteToken);

        // 3. Register with Invite
        var registerWithInviteRequest = new {
            InvitationToken = inviteToken,
            Nome = "New Client",
            Email = "newclient@test.com",
            Senha = "ClientPassword123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register-with-invite", registerWithInviteRequest);
        registerResponse.EnsureSuccessStatusCode();

        var registerData = await registerResponse.Content.ReadFromJsonAsync<ApiResponseDTO<UserRegistrationResponseDTO>>();
        Assert.True(registerData?.Sucesso);
        Assert.Equal("newclient@test.com", registerData.Dados.Email);
    }
}
