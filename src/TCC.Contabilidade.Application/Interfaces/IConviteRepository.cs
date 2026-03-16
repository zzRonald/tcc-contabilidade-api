using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IConviteRepository
{
    Task AdicionarAsync(Convite convite);
    Task<Convite?> ObterPorTokenAsync(string token);
    Task<List<Convite>> GetByContadorId(Guid contadorId);
    Task SalvarAlteracoesAsync();
}

   