using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class CompetenciaIntegrationTests : BaseIntegrationTest
{
    public CompetenciaIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAndListCompetencias_ReturnsSuccess()
    {
        // Arrange
        var empresa = await CreateEmpresaAsync();
        var (_, user) = await AuthenticateAsync(TipoUsuario.Contador, empresa.Id);
        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        var request = new CreateCompetenciaDto(empresa.Id, 12, 2023, "Teste");

        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/competencias", request);

        // Assert
        if (!createResponse.IsSuccessStatusCode)
        {
            var errorBody = await createResponse.Content.ReadAsStringAsync();
            throw new Exception($"Create Competencia failed with status {createResponse.StatusCode}. Body: {errorBody}");
        }
        createResponse.EnsureSuccessStatusCode();

        var listResponse = await Client.GetAsync($"/api/competencias/empresa/{empresa.Id}");
        listResponse.EnsureSuccessStatusCode();

        var result = await listResponse.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<CompetenciaResponseDto>>>();
        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados);
        Assert.Contains(result.Dados!, c => c.Mes == 12 && c.Ano == 2023);
    }
}
