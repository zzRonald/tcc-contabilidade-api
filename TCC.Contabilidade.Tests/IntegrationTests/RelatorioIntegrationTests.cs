using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Relatorios;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class RelatorioIntegrationTests : BaseIntegrationTest
{
    public RelatorioIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetRelatorioMensal_ReturnsSuccess()
    {
        // Arrange
        var empresa = await CreateEmpresaAsync();
        var (_, user) = await AuthenticateAsync(TipoUsuario.Contador, empresa.Id);
        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        // Act
        var response = await Client.GetAsync($"/api/relatorios/mensal?empresaId={empresa.Id}&mes=10&ano=2023");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<RelatorioMensalDTO>>();
        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados);
    }
}
