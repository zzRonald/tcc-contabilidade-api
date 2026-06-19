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
            .Include(g => g.Comprovante)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<(IEnumerable<GuiaPagamento> Itens, int Total)> ObterPaginadoAsync(
        Guid empresaId,
        int pagina,
        int tamanhoPagina,
        Guid? competenciaId = null,
        bool? apenasVencidas = null,
        bool? apenasAVencer = null)
    {
        var query = _context.GuiasPagamento
            .Include(g => g.Competencia)
            .Include(g => g.Documento)
            .Include(g => g.Comprovante)
            .Where(g => g.EmpresaId == empresaId);

        if (competenciaId.HasValue)
        {
            query = query.Where(g => g.CompetenciaId == competenciaId.Value);
        }

        var hoje = DateTime.UtcNow;

        if (apenasVencidas == true)
        {
            query = query.Where(g => g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Pago &&
                                   g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Cancelado &&
                                   g.DataVencimento < hoje);
        }
        else if (apenasAVencer == true)
        {
            query = query.Where(g => g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Pago &&
                                   g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Cancelado &&
                                   g.DataVencimento >= hoje);
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

    public async Task<int> CountVencidasByUsuarioIdAsync(Guid usuarioId)
    {
        var hoje = DateTime.UtcNow;

        return await _context.GuiasPagamento
            .Join(_context.UsuariosEmpresas,
                g => g.EmpresaId,
                ue => ue.EmpresaId,
                (g, ue) => new { Guia = g, UsuarioEmpresa = ue })
            .Where(x => x.UsuarioEmpresa.UsuarioId == usuarioId &&
                        x.Guia.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Pago &&
                        x.Guia.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Cancelado &&
                        x.Guia.DataVencimento < hoje)
            .Select(x => x.Guia.Id)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusGuia? status = null, bool? apenasVencidas = null)
    {
        var query = _context.GuiasPagamento
            .Where(g => g.CompetenciaId == competenciaId);

        if (status.HasValue)
        {
            query = query.Where(g => g.Status == status.Value);
        }

        if (apenasVencidas == true)
        {
            var hoje = DateTime.UtcNow;
            query = query.Where(g => g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Pago &&
                                   g.Status != TCC.Contabilidade.Domain.Enums.StatusGuia.Cancelado &&
                                   g.DataVencimento < hoje);
        }

        return await query.CountAsync();
    }
}
