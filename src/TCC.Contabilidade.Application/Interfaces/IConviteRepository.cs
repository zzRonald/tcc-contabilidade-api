using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IConviteRepository
{
    Task AdicionarAsync(Convite convite);
    Task<Convite?> ObterPorTokenAsync(string token);
    Task<List<Convite>> GetByContadorId(Guid contadorId);
    Task<(List<Convite> Items, int TotalCount)> GetPagedByContadorId(Guid contadorId, int page, int pageSize);
    Task SalvarAlteracoesAsync();
}

   