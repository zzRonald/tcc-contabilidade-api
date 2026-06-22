using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class OperacionalIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public OperacionalIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(string Token, Guid EmpresaId)> SetupEmpresaAsync()
    {
        var email = $"contador_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);
        Guid empresaId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User("Contador User", email, senhaHash, TipoUsuario.Contador);
            db.Usuarios.Add(user);

            var empresa = new Empresa { Nome = "Empresa Operacional", CNPJ = "07255931000188" }; // CNPJ Válido
            db.Empresas.Add(empresa);

            await db.SaveChangesAsync();

            db.UsuariosEmpresas.Add(new UsuarioEmpresa { UsuarioId = user.Id, EmpresaId = empresa.Id });
            await db.SaveChangesAsync();

            empresaId = empresa.Id;
        }

        var loginRequest = new { Email = email, Senha = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        return (result!.Dados!.AccessToken, empresaId);
    }

    [Fact]
    public async Task OperacionalFlow_ReturnsSuccess()
    {
        var (token, empresaId) = await SetupEmpresaAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 1. Criar Competência
        var competenciaRequest = new { EmpresaId = empresaId, Mes = 10, Ano = 2023 };
        var compResp = await _client.PostAsJsonAsync("/api/competencias", competenciaRequest);

        if (compResp.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var error = await compResp.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
            throw new Exception($"Create Competencia failed: {error?.Mensagem}");
        }

        compResp.EnsureSuccessStatusCode();

        // Obter a competência criada para os próximos passos
        var listCompResp = await _client.GetAsync($"/api/competencias/empresa/{empresaId}");
        var listCompResult = await listCompResp.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<CompetenciaResponseDto>>>();
        var competenciaId = listCompResult!.Dados!.First().Id;

        // 2. Solicitação de Documento
        var solicitacaoRequest = new
        {
            EmpresaId = empresaId,
            CompetenciaId = competenciaId,
            Titulo = "Extrato Outubro",
            Descricao = "Enviar extrato PDF",
            TipoDocumento = (int)TipoDocumento.ExtratoBancario
        };
        var solResp = await _client.PostAsJsonAsync("/api/solicitacaodocumento", solicitacaoRequest);
        solResp.EnsureSuccessStatusCode();

        // 3. Criação de Guia de Pagamento
        var guiaRequest = new
        {
            EmpresaId = empresaId,
            CompetenciaId = competenciaId,
            Tipo = (int)TipoGuia.DAS,
            Valor = 150.50m,
            DataVencimento = DateTime.UtcNow.AddDays(5)
        };
        var guiaResp = await _client.PostAsJsonAsync("/api/guiaspagamento", guiaRequest);
        guiaResp.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, guiaResp.StatusCode);
    }
}
