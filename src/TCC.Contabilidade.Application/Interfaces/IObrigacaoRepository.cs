using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IObrigacaoRepository
{
    Task<Obrigacao?> ObterPorIdAsync(Guid id);
    Task<(IEnumerable<Obrigacao> Itens, int Total)> ObterPaginadoAsync(Guid empresaId, int pagina, int tamanhoPagina, Guid? competenciaId = null);
    Task AdicionarAsync(Obrigacao obrigacao);
    Task AtualizarAsync(Obrigacao obrigacao);
    Task RemoverAsync(Obrigacao obrigacao);
    Task SalvarAlteracoesAsync();
    Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusObrigacao? status = null, bool? apenasAtrasadas = null);
    Task<List<Obrigacao>> ObterObrigacoesVencimentoProximoAsync(int dias);
}
