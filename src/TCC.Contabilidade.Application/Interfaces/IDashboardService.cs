using TCC.Contabilidade.Application.DTO;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDTO> GetResumoAsync(Guid usuarioId);
}
