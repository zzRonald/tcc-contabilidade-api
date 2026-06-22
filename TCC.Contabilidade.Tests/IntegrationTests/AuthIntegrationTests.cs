using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

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
    public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = new User("Test User", email, senhaHash, TipoUsuario.Admin);
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
        }

        var loginRequest = new { Email = email, Senha = password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();

        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados!.AccessToken);
        Assert.Equal(email, result.Dados.Usuario!.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { Email = $"wrong_{Guid.NewGuid()}@example.com", Senha = "wrongpassword" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
        Assert.False(result!.Sucesso);
    }
}
