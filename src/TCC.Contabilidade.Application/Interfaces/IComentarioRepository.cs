using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IComentarioRepository
{
    Task AddAsync(Comentario comentario);
    Task<List<Comentario>> GetByDocumentoIdAsync(Guid documentoId);
    Task<List<Comentario>> GetByGuiaPagamentoIdAsync(Guid guiaPagamentoId);
    Task SaveChangesAsync();
}
