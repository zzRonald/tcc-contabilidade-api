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

public class DashboardIntegrationTests : BaseIntegrationTest
{
    public DashboardIntegrationTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsData()
    {
        // Arrange
        var empresa = await CreateEmpresaAsync();
        var (_, user) = await AuthenticateAsync(TipoUsuario.Contador, empresa.Id);
        await VincularUsuarioEmpresaAsync(user.Id, empresa.Id);

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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

        // Act
        var response = await Client.GetAsync("/api/dashboard/resumo");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<DashboardSummaryDTO>>();
        Assert.True(result!.Sucesso);
        Assert.NotNull(result.Dados);
        Assert.True(result.Dados.TotalEmpresas >= 1);
    }
}
