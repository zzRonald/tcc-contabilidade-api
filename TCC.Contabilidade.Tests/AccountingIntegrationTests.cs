using System.Net.Http.Headers;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.API.Controllers;
using TCC.Contabilidade.Domain.Enums;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class AccountingIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public AccountingIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(string token, Guid companyId, Guid competencyId)> SetupBaseData()
    {
        // 1. Authenticate
        var email = $"contador_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new AuthController.RegisterRequest("Test Contador", email, "Pass123!", "Contador"));

        // 2. Create Company (will have no tenant context yet)
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new AuthController.LoginRequest(email, "Pass123!"));
        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Dados.AccessToken);

        var cnpj = Guid.NewGuid().ToString().Substring(0, 14);
        await _client.PostAsJsonAsync("/api/Empresas", new CreateEmpresaDto { Nome = "Test Co", CNPJ = cnpj });

        // 3. Login again to get tenantId in token
        loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new AuthController.LoginRequest(email, "Pass123!"));
        loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        var token = loginData.Dados.AccessToken;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var companies = await _client.GetFromJsonAsync<ApiResponseDTO<IEnumerable<EmpresaResponseDto>>>("/api/Empresas");
        var companyId = companies.Dados.First().Id;

        // 4. Create Competency
        await _client.PostAsJsonAsync("/api/Competencias", new CreateCompetenciaDto(companyId, 1, 2024, null));
        var competencies = await _client.GetFromJsonAsync<ApiResponseDTO<IEnumerable<CompetenciaResponseDto>>>($"/api/Competencias/empresa/{companyId}");
        var competencyId = competencies.Dados.First().Id;

        return (token, companyId, competencyId);
    }

    [Fact]
    public async Task DocumentRequest_Flow_ShouldSucceed()
    {
        var (token, companyId, competencyId) = await SetupBaseData();

        // Create Document Request
        var requestDto = new CreateSolicitacaoDocumentoDto(
            companyId,
            competencyId,
            TipoDocumento.NotaFiscal,
            "Favor enviar NF"
        );

        var response = await _client.PostAsJsonAsync("/api/SolicitacaoDocumento", requestDto);
        response.EnsureSuccessStatusCode();

        // Verify
        var listResponse = await _client.GetFromJsonAsync<ApiResponseDTO<IEnumerable<SolicitacaoDocumentoResponseDto>>>($"/api/SolicitacaoDocumento/empresa/{companyId}");
        Assert.Contains(listResponse.Dados, s => s.CompetenciaId == competencyId && s.TipoDocumento == TipoDocumento.NotaFiscal);
    }

    [Fact]
    public async Task PaymentGuide_Flow_ShouldSucceed()
    {
        var (token, companyId, competencyId) = await SetupBaseData();

        // Create Payment Guide
        var guideDto = new GuiaPagamentoRequestDTO(
            competencyId,
            TipoGuia.DAS,
            1500.50m,
            DateTime.UtcNow.AddDays(10),
            "Guia de Teste",
            null
        );

        var response = await _client.PostAsJsonAsync("/api/GuiasPagamento", guideDto);
        response.EnsureSuccessStatusCode();

        // Verify
        var listResponse = await _client.GetFromJsonAsync<ApiResponseDTO<IEnumerable<GuiaPagamentoResponseDTO>>>("/api/GuiasPagamento");
        Assert.Contains(listResponse.Dados, g => g.Valor == 1500.50m && g.Tipo == TipoGuia.DAS);
    }
}
