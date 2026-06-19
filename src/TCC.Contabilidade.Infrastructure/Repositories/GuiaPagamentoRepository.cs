using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class GuiaPagamentoRepository : IGuiaPagamentoRepository
{
    private readonly AppDbContext _context;

    public GuiaPagamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GuiaPagamento?> ObterPorIdAsync(Guid id)
    {
        return await _context.GuiasPagamento
            .Include(g => g.Competencia)
            .Include(g => g.Documento)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<(IEnumerable<GuiaPagamento> Itens, int Total)> ObterPaginadoAsync(Guid empresaId, int pagina, int tamanhoPagina, Guid? competenciaId = null)
    {
        var query = _context.GuiasPagamento
            .Include(g => g.Competencia)
            .Include(g => g.Documento)
            .Where(g => g.EmpresaId == empresaId);

        if (competenciaId.HasValue)
        {
            query = query.Where(g => g.CompetenciaId == competenciaId.Value);
        }

        var total = await query.CountAsync();
        var itens = await query
            .OrderByDescending(g => g.DataVencimento)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(GuiaPagamento guia)
    {
        await _context.GuiasPagamento.AddAsync(guia);
    }

    public async Task AtualizarAsync(GuiaPagamento guia)
    {
        _context.GuiasPagamento.Update(guia);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(GuiaPagamento guia)
    {
        _context.GuiasPagamento.Remove(guia);
        await Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
