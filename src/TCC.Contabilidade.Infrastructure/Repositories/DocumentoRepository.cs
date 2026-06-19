using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly AppDbContext _context;

    public DocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Documento documento)
    {
        await _context.Documentos.AddAsync(documento);
    }

    public async Task<Documento?> GetByIdAsync(Guid id)
    {
        return await _context.Documentos.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusDocumento? status = null)
    {
        var query = _context.Documentos
            .Where(d => d.CompetenciaId == competenciaId);

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        return await query.CountAsync();
    }
}
