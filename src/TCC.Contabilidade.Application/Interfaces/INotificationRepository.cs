using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetByEmpresaIdAsync(Guid empresaId);
    Task<List<Notification>> GetByReferenciaIdAsync(Guid referenciaId, TCC.Contabilidade.Domain.Enums.TipoNotificacao tipo);
    Task SaveChangesAsync();
}
