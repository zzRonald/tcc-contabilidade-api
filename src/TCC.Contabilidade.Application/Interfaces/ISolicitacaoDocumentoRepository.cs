using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface ISolicitacaoDocumentoRepository
{
    Task<SolicitacaoDocumento?> GetByIdAsync(Guid id);
    Task<(IEnumerable<SolicitacaoDocumento> Items, int TotalCount)> GetPagedByEmpresaIdAsync(Guid empresaId, int page, int pageSize);
    Task<(IEnumerable<SolicitacaoDocumento> Items, int TotalCount)> GetPagedByCompetenciaIdAsync(Guid competenciaId, int page, int pageSize);
    Task AddAsync(SolicitacaoDocumento solicitacao);
    Task UpdateAsync(SolicitacaoDocumento solicitacao);
    Task<int> CountByCompetenciaIdAsync(Guid competenciaId);
}
