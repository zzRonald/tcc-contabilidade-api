using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class ComentarioRepository : IComentarioRepository
{
    private readonly AppDbContext _context;

    public ComentarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Comentario comentario)
    {
        await _context.Set<Comentario>().AddAsync(comentario);
    }

    public async Task<List<Comentario>> GetByDocumentoIdAsync(Guid documentoId)
    {
        return await _context.Set<Comentario>()
            .Include(c => c.Usuario)
            .Where(c => c.DocumentoId == documentoId)
            .OrderBy(c => c.DataCriacao)
            .ToListAsync();
    }

    public async Task<List<Comentario>> GetByGuiaPagamentoIdAsync(Guid guiaPagamentoId)
    {
        return await _context.Set<Comentario>()
            .Include(c => c.Usuario)
            .Where(c => c.GuiaPagamentoId == guiaPagamentoId)
            .OrderBy(c => c.DataCriacao)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
