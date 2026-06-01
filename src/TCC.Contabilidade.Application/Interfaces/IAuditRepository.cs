using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog);
    Task SaveChangesAsync();
}
