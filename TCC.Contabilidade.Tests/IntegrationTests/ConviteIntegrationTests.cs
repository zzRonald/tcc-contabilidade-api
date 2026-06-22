using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class ConviteIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public ConviteIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsContadorAsync()
    {
        var email = $"contador_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User("Contador User", email, senhaHash, TipoUsuario.Contador);
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
        }

        var loginRequest = new { Email = email, Senha = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        return result!.Dados!.AccessToken;
    }

    [Fact]
    public async Task InviteAndRegisterFlow_ReturnsSuccess()
    {
        // 1. Criar Convite
        var token = await AuthenticateAsContadorAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var clientEmail = $"cliente_{Guid.NewGuid()}@example.com";
        var inviteRequest = new { EmailCliente = clientEmail };
        var inviteResponse = await _client.PostAsJsonAsync("/api/convites", inviteRequest);
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

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register-with-invite", registerRequest);

        // Assert
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<ApiResponseDTO<UserRegistrationResponseDTO>>();
        Assert.True(registerResult!.Sucesso);
        Assert.Equal("Cliente registrado com sucesso", registerResult.Mensagem);
    }
}
