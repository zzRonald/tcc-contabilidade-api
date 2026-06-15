using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IDocumentoRepository
{
    Task AddAsync(Documento documento);
    Task<Documento?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}
