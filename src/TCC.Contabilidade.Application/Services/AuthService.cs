using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO?> LoginAsync(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            return null;

        return await GenerateAuthResponseAsync(usuario);
    }

    public async Task<AuthResponseDTO?> RefreshTokenAsync(string token)
    {
        var refreshToken = await _refreshTokenRepository.ObterPorTokenAsync(token);

        if (refreshToken == null || !refreshToken.IsAtivo)
            return null;

        // Revoga o token atual
        refreshToken.RevogadoEm = DateTime.UtcNow;
        await _refreshTokenRepository.AtualizarAsync(refreshToken);

        // Gera novos tokens
        return await GenerateAuthResponseAsync(refreshToken.Usuario!);
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await _refreshTokenRepository.ObterPorTokenAsync(token);

        if (refreshToken == null || !refreshToken.IsAtivo)
            return false;

        refreshToken.RevogadoEm = DateTime.UtcNow;
        await _refreshTokenRepository.AtualizarAsync(refreshToken);
        await _refreshTokenRepository.SalvarAlteracoesAsync();

        return true;
    }

    private async Task<AuthResponseDTO> GenerateAuthResponseAsync(User usuario)
    {
        var accessToken = _tokenService.GenerateAccessToken(usuario);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenStr,
            UsuarioId = usuario.Id,
            ExpiraEm = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AdicionarAsync(refreshToken);
        await _refreshTokenRepository.SalvarAlteracoesAsync();

        return new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            ExpiraEm = 900, // 15 minutos em segundos
            Usuario = new UserResponseDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario.ToString()
            }
        };
    }
}
