using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Utils;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public AuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<AuditLogResponseDTO> Logs, int Total)> GetPagedAsync(AuditLogFilterDTO filtros)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (filtros.DataInicio.HasValue)
            query = query.Where(a => a.DataHora >= filtros.DataInicio.Value);

        if (filtros.DataFim.HasValue)
            query = query.Where(a => a.DataHora <= filtros.DataFim.Value);

        if (filtros.UsuarioId.HasValue)
            query = query.Where(a => a.UsuarioId == filtros.UsuarioId.Value);

        if (!string.IsNullOrWhiteSpace(filtros.Acao))
            query = query.Where(a => a.Acao.Contains(filtros.Acao));

        var total = await query.CountAsync();

        var logs = await query
            .OrderByDescending(a => a.DataHora)
            .Skip((filtros.Pagina - 1) * filtros.TamanhoPagina)
            .Take(filtros.TamanhoPagina)
            .Select(audit => new
            {
                audit.Id,
                audit.UsuarioId,
                UsuarioNome = _context.Usuarios.Where(u => u.Id == audit.UsuarioId).Select(u => u.Nome).FirstOrDefault(),
                audit.Acao,
                audit.Entidade,
                audit.EntidadeId,
                audit.DataHora,
                audit.Ip
            })
            .ToListAsync();

        var logsDtos = logs.Select(audit => new AuditLogResponseDTO
        {
            Id = audit.Id,
            UsuarioId = audit.UsuarioId,
            UsuarioNome = audit.UsuarioNome,
            Acao = audit.Acao,
            Entidade = audit.Entidade,
            EntidadeId = audit.EntidadeId,
            DataHora = audit.DataHora,
            Ip = PrivacyUtils.MaskIp(audit.Ip)
        }).ToList();

        return (logsDtos, total);
    }
}
