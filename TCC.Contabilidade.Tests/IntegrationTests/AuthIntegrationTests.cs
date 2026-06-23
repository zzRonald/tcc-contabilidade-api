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

public class AuthIntegrationTests : BaseIntegrationTest
{
    public AuthIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = new User("Test User", email, senhaHash, TipoUsuario.Admin);
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
        }

        var loginRequest = new { Email = email, Senha = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

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
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
        Assert.False(result!.Sucesso);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var registerRequest = new
        {
            Nome = "Novo Usuario",
            Email = $"novo_{Guid.NewGuid()}@example.com",
            Senha = "Password123!",
            Perfil = "Cliente"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<UserRegistrationResponseDTO>>();
        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados);
        Assert.Equal(registerRequest.Email, result.Dados.Email);
    }
}
