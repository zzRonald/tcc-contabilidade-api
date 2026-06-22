using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class EmpresaIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public EmpresaIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsync()
    {
        var email = $"admin_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User("Admin User", email, senhaHash, TipoUsuario.Admin);
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
        }

        var loginRequest = new { Email = email, Senha = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        return result!.Dados!.AccessToken;
    }

    [Fact]
    public async Task CreateEmpresa_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await AuthenticateAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateEmpresaDto
        {
            Nome = "Empresa Teste",
            CNPJ = "44509539000180" // CNPJ Válido
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/empresas", createRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
        Assert.True(result!.Sucesso);
        Assert.Equal("Empresa criada com sucesso", result.Mensagem);
    }
}
