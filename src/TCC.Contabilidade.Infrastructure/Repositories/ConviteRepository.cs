using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class ConviteRepository : IConviteRepository
{
    private readonly AppDbContext _context;

    public ConviteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Convite convite)
    {
        await _context.Convites.AddAsync(convite);
    }

    public async Task<Convite?> ObterPorTokenAsync(string token)
    {
        return await _context.Convites
            .FirstOrDefaultAsync(c => c.Token == token);
    }

    public async Task<List<Convite>> GetByContadorId(Guid contadorId)
    {
        return await _context.Convites
            .Where(c => c.ContadorId == contadorId)
            .ToListAsync();
    }

    public async Task<(List<Convite> Items, int TotalCount)> GetPagedByContadorId(Guid contadorId, int page, int pageSize)
    {
        var query = _context.Convites
            .AsNoTracking()
            .Where(c => c.ContadorId == contadorId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}