using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class ObrigacaoRepository : IObrigacaoRepository
{
    private readonly AppDbContext _context;

    public ObrigacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Obrigacao?> ObterPorIdAsync(Guid id)
    {
        return await _context.Obrigacoes
            .Include(o => o.Competencia)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(IEnumerable<Obrigacao> Itens, int Total)> ObterPaginadoAsync(Guid empresaId, int pagina, int tamanhoPagina, Guid? competenciaId = null)
    {
        var query = _context.Obrigacoes
            .Include(o => o.Competencia)
            .Where(o => o.EmpresaId == empresaId);

        if (competenciaId.HasValue)
        {
            query = query.Where(o => o.CompetenciaId == competenciaId.Value);
        }

        var total = await query.CountAsync();
        var itens = await query
            .OrderBy(o => o.DataVencimento)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(Obrigacao obrigacao)
    {
        await _context.Obrigacoes.AddAsync(obrigacao);
    }

    public async Task AtualizarAsync(Obrigacao obrigacao)
    {
        _context.Obrigacoes.Update(obrigacao);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(Obrigacao obrigacao)
    {
        _context.Obrigacoes.Remove(obrigacao);
        await Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusObrigacao? status = null, bool? apenasAtrasadas = null)
    {
        var query = _context.Obrigacoes
            .Where(o => o.CompetenciaId == competenciaId);

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (apenasAtrasadas == true)
        {
            var hoje = DateTime.UtcNow;
            query = query.Where(o => o.Status != TCC.Contabilidade.Domain.Enums.StatusObrigacao.Concluida &&
                                   o.Status != TCC.Contabilidade.Domain.Enums.StatusObrigacao.Cancelada &&
                                   o.DataVencimento < hoje);
        }

        return await query.CountAsync();
    }

    public async Task<List<Obrigacao>> ObterObrigacoesVencimentoProximoAsync(int dias)
    {
        var hoje = DateTime.UtcNow.Date;
        var dataLimite = hoje.AddDays(dias);

        return await _context.Obrigacoes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(o => o.Empresa)
            .Where(o => o.Status != TCC.Contabilidade.Domain.Enums.StatusObrigacao.Concluida &&
                       o.Status != TCC.Contabilidade.Domain.Enums.StatusObrigacao.Cancelada &&
                       o.DataVencimento.Date >= hoje &&
                       o.DataVencimento.Date <= dataLimite)
            .ToListAsync();
    }
}
