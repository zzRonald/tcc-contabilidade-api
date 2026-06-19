using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class SolicitacaoDocumentoRepository : ISolicitacaoDocumentoRepository
{
    private readonly AppDbContext _context;

    public SolicitacaoDocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitacaoDocumento?> GetByIdAsync(Guid id)
    {
        return await _context.SolicitacoesDocumentos
            .Include(s => s.Empresa)
            .Include(s => s.Competencia)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<(IEnumerable<SolicitacaoDocumento> Items, int TotalCount)> GetPagedByEmpresaIdAsync(Guid empresaId, int page, int pageSize)
    {
        var query = _context.SolicitacoesDocumentos
            .Where(s => s.EmpresaId == empresaId)
            .OrderByDescending(s => s.DataCriacao);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<SolicitacaoDocumento> Items, int TotalCount)> GetPagedByCompetenciaIdAsync(Guid competenciaId, int page, int pageSize)
    {
        var query = _context.SolicitacoesDocumentos
            .Where(s => s.CompetenciaId == competenciaId)
            .OrderByDescending(s => s.DataCriacao);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(SolicitacaoDocumento solicitacao)
    {
        await _context.SolicitacoesDocumentos.AddAsync(solicitacao);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SolicitacaoDocumento solicitacao)
    {
        _context.SolicitacoesDocumentos.Update(solicitacao);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountByCompetenciaIdAsync(Guid competenciaId)
    {
        return await _context.SolicitacoesDocumentos
            .CountAsync(s => s.CompetenciaId == competenciaId);
    }
}
