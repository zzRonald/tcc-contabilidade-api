using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class DashboardIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public DashboardIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsContadorWithDataAsync()
    {
        var email = $"contador_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User("Contador User", email, senhaHash, TipoUsuario.Contador);
            db.Usuarios.Add(user);

            var empresa = new Empresa { Nome = "Empresa Dashboard", CNPJ = "07255931000188" }; // CNPJ Válido
            db.Empresas.Add(empresa);
            await db.SaveChangesAsync();

            db.UsuariosEmpresas.Add(new UsuarioEmpresa { UsuarioId = user.Id, EmpresaId = empresa.Id });

            // Adicionar dados para o dashboard contar
            db.GuiasPagamento.Add(new GuiaPagamento
            {
                EmpresaId = empresa.Id,
                Tipo = TipoGuia.DAS,
                Status = StatusGuia.Pendente,
                DataVencimento = DateTime.UtcNow.AddDays(5),
                Valor = 100
            });

            await db.SaveChangesAsync();
        }

        var loginRequest = new { Email = email, Senha = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        return result!.Dados!.AccessToken;
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsData()
    {
        // Arrange
        var token = await AuthenticateAsContadorWithDataAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dashboard/resumo");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<DashboardSummaryDTO>>();
        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados);
        Assert.True(result.Dados.TotalEmpresas >= 1);
    }
}
