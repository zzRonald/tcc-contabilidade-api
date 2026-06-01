using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User usuario);
    string GenerateRefreshToken();
}
