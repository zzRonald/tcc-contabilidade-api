using System.Net.Http.Headers;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.API.Controllers;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class CompanyIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public CompanyIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsAdmin(string email)
    {
        var registerRequest = new AuthController.RegisterRequest(
            "Test Admin",
            email,
            "StrongPassword123!",
            "Admin"
        );

        await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        var loginRequest = new AuthController.LoginRequest(email, "StrongPassword123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();

        return loginData.Dados.AccessToken;
    }

    [Fact]
    public async Task CreateCompany_And_CreateCompetency_ShouldSucceed()
    {
        // 1. Authenticate
        var email = $"admin_{Guid.NewGuid()}@test.com";
        var token = await AuthenticateAsAdmin(email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create Company
        var cnpj = "12345678000199";
        var createEmpresaDto = new CreateEmpresaDto {
            Nome = "Test Company",
            CNPJ = cnpj
        };

        var companyResponse = await _client.PostAsJsonAsync("/api/Empresas", createEmpresaDto);
        companyResponse.EnsureSuccessStatusCode();

        // 3. Login again to get tenantId in token
        var loginResponse2 = await _client.PostAsJsonAsync("/api/Auth/login", new AuthController.LoginRequest(email, "StrongPassword123!"));
        var loginData2 = await loginResponse2.Content.ReadFromJsonAsync<ApiResponseDTO<AuthResponseDTO>>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData2.Dados.AccessToken);

        // 4. List Companies to get ID
        var listResponse = await _client.GetAsync("/api/Empresas");
        listResponse.EnsureSuccessStatusCode();
        var listData = await listResponse.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<EmpresaResponseDto>>>();

        var company = listData.Dados.FirstOrDefault();
        Assert.NotNull(company);
        var companyId = company.Id;

        // 5. Create Competency
        var createCompetenciaDto = new CreateCompetenciaDto(companyId, 5, 2024, "Competência de Teste");

        var competenciaResponse = await _client.PostAsJsonAsync("/api/Competencias", createCompetenciaDto);
        competenciaResponse.EnsureSuccessStatusCode();

        // 6. Verify Competency
        var competenciasResponse = await _client.GetAsync($"/api/Competencias/empresa/{companyId}");
        competenciasResponse.EnsureSuccessStatusCode();
        var competenciasData = await competenciasResponse.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<CompetenciaResponseDto>>>();
        Assert.Contains(competenciasData.Dados, c => c.Mes == 5 && c.Ano == 2024);
    }
}
