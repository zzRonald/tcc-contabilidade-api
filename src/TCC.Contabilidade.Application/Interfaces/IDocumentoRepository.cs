using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IDocumentoRepository
{
    Task AddAsync(Documento documento);
    Task<Documento?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task<int> CountByCompetenciaIdAsync(Guid competenciaId, TCC.Contabilidade.Domain.Enums.StatusDocumento? status = null);
}
