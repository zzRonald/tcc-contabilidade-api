using System.Net;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class EmpresaIntegrationTests : BaseIntegrationTest
{
    public EmpresaIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateEmpresa_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync();

        var createRequest = new CreateEmpresaDto
        {
            Nome = "Empresa Teste",
            CNPJ = "44509539000180" // CNPJ Válido
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/empresas", createRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
        Assert.True(result!.Sucesso);
        Assert.Equal("Empresa criada com sucesso", result.Mensagem);
    }

    [Fact]
    public async Task GetEmpresas_ReturnsList()
    {
        // Arrange
        var (_, user) = await AuthenticateAsync();
        var empresa = await CreateEmpresaAsync();

        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        // Act
        var response = await Client.GetAsync("/api/empresas");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<EmpresaResponseDto>>>();
        Assert.True(result!.Sucesso);
        Assert.NotEmpty(result.Dados!);
    }

    [Fact]
    public async Task UpdateEmpresa_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var (_, user) = await AuthenticateAsync();
        var empresa = await CreateEmpresaAsync();
        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        var updateRequest = new UpdateEmpresaDto
        {
            Nome = "Empresa Atualizada",
            CNPJ = empresa.CNPJ
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/empresas/{empresa.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<object>>();
        Assert.True(result!.Sucesso);
        Assert.Equal("Empresa atualizada com sucesso", result.Mensagem);
    }
}
