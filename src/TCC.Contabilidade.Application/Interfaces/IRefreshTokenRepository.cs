using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorTokenAsync(string token);
    Task AdicionarAsync(RefreshToken refreshToken);
    Task AtualizarAsync(RefreshToken refreshToken);
    Task SalvarAlteracoesAsync();
}
