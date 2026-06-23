using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Tests.Fixtures;
using Xunit;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public class SolicitacaoDocumentoIntegrationTests : BaseIntegrationTest
{
    public SolicitacaoDocumentoIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAndListSolicitacao_ReturnsSuccess()
    {
        // Arrange
        var empresa = await CreateEmpresaAsync();
        var (_, user) = await AuthenticateAsync(TipoUsuario.Contador, empresa.Id);
        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        // Criar competência primeiro
        var compRequest = new TCC.Contabilidade.Application.DTO.Competencias.CreateCompetenciaDto(empresa.Id, 11, 2023, null);
        var compResp = await Client.PostAsJsonAsync("/api/competencias", compRequest);
        compResp.EnsureSuccessStatusCode();

        var listCompResp = await Client.GetAsync($"/api/competencias/empresa/{empresa.Id}");
        var listCompResult = await listCompResp.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<TCC.Contabilidade.Application.DTO.Competencias.CompetenciaResponseDto>>>();
        var competenciaId = listCompResult!.Dados!.First().Id;

        var request = new CreateSolicitacaoDocumentoDto(
            empresa.Id,
            competenciaId,
            TipoDocumento.NotaFiscal,
            "Solicitação de teste"
        );

        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/solicitacaodocumento", request);
        createResponse.EnsureSuccessStatusCode();

        var listResponse = await Client.GetAsync($"/api/solicitacaodocumento/empresa/{empresa.Id}");
        listResponse.EnsureSuccessStatusCode();

        // Assert
        var result = await listResponse.Content.ReadFromJsonAsync<ApiResponseDTO<IEnumerable<SolicitacaoDocumentoResponseDto>>>();
        Assert.True(result!.Sucesso);
        Assert.NotEmpty(result.Dados!);
    }
}
