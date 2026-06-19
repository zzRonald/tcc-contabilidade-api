using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IGuiaPagamentoRepository
{
    Task<GuiaPagamento?> ObterPorIdAsync(Guid id);
    Task<(IEnumerable<GuiaPagamento> Itens, int Total)> ObterPaginadoAsync(
        Guid empresaId,
        int pagina,
        int tamanhoPagina,
        Guid? competenciaId = null,
        bool? apenasVencidas = null,
        bool? apenasAVencer = null);
    Task AdicionarAsync(GuiaPagamento guia);
    Task AtualizarAsync(GuiaPagamento guia);
    Task RemoverAsync(GuiaPagamento guia);
    Task SalvarAlteracoesAsync();
    Task<int> CountVencidasByUsuarioIdAsync(Guid usuarioId);
    Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusGuia? status = null, bool? apenasVencidas = null);
}
