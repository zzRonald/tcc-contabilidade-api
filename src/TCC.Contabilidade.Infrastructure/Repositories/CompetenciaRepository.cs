using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class CompetenciaRepository : ICompetenciaRepository
{
    private readonly AppDbContext _context;

    public CompetenciaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Competencia?> GetByIdAsync(Guid id)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Competencia?> GetByMesAnoAsync(Guid empresaId, int mes, int ano)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Mes == mes && c.Ano == ano);
    }

    public async Task<(List<Competencia> Items, int TotalCount)> GetPagedByEmpresaIdAsync(Guid empresaId, int page, int pageSize)
    {
        var query = _context.Competencias
            .Where(c => c.EmpresaId == empresaId)
            .OrderByDescending(c => c.Ano)
            .ThenByDescending(c => c.Mes);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Competencia competencia)
    {
        await _context.Competencias.AddAsync(competencia);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Competencia competencia)
    {
        _context.Competencias.Update(competencia);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid empresaId, int mes, int ano)
    {
        return await _context.Competencias
            .AnyAsync(c => c.EmpresaId == empresaId && c.Mes == mes && c.Ano == ano);
    }
}
