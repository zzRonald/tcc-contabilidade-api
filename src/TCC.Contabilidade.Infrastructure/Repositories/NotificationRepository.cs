using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task<List<Notification>> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await _context.Notifications
            .Where(n => n.EmpresaId == empresaId)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetByReferenciaIdAsync(Guid referenciaId, TCC.Contabilidade.Domain.Enums.TipoNotificacao tipo)
    {
        return await _context.Notifications
            .IgnoreQueryFilters()
            .Where(n => n.ReferenciaId == referenciaId && n.Tipo == tipo)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
