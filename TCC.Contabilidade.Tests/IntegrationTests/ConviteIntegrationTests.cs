using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class ConviteIntegrationTests : BaseIntegrationTest
{
    public ConviteIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task InviteAndRegisterFlow_ReturnsSuccess()
    {
        // 1. Criar Convite
        await AuthenticateAsync(TipoUsuario.Contador);

        var clientEmail = $"cliente_{Guid.NewGuid()}@example.com";
        var inviteRequest = new { EmailCliente = clientEmail };
        var inviteResponse = await Client.PostAsJsonAsync("/api/convites", inviteRequest);

        inviteResponse.EnsureSuccessStatusCode();

        var inviteResult = await inviteResponse.Content.ReadFromJsonAsync<JsonElement>();
        string invitationToken = inviteResult.GetProperty("token").GetString()!;

        // 2. Registrar via Convite
        var registerRequest = new
        {
            InvitationToken = invitationToken,
            Nome = "Cliente Teste",
            Email = clientEmail,
            Senha = "Password123!"
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register-with-invite", registerRequest);

        // Assert
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<ApiResponseDTO<UserRegistrationResponseDTO>>();
        Assert.True(registerResult!.Sucesso);
        Assert.Equal("Cliente registrado com sucesso", registerResult.Mensagem);
    }

    [Fact]
    public async Task RegisterWithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            InvitationToken = "invalid-token",
            Nome = "Cliente Teste",
            Email = "teste@exemplo.com",
            Senha = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register-with-invite", registerRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
