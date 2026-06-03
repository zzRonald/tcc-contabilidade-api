using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog);
    Task SaveChangesAsync();
    Task<(IEnumerable<AuditLogResponseDTO> Logs, int Total)> GetPagedAsync(AuditLogFilterDTO filtros);
}
