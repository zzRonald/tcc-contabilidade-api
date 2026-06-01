using Microsoft.AspNetCore.Mvc;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTOs;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public AuthController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            User usuario = await _userService.RegisterAsync(
                request.Nome,
                request.Email,
                request.Senha,
                request.Perfil
            );

            return Ok(ApiResponseDTO<object>.Success(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                TipoUsuario = usuario.TipoUsuario.ToString()
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPost("register-with-invite")]
    public async Task<IActionResult> RegisterWithInvite(
    [FromBody] RegisterWithInviteRequest request)
    {
        try
        {
            var user = await _userService.RegisterWithInviteAsync(
                request.InvitationToken,
                request.Nome,
                request.Email,
                request.Senha
            );

            return Ok(ApiResponseDTO<object>.Success(new
            {
                user.Id,
                user.Email
            }, "Cliente registrado com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request.Email, request.Senha);

        if (response == null)
            return Unauthorized(ApiResponseDTO<object>.Fail("Email ou senha inválidos", 401));

        return Ok(ApiResponseDTO<AuthResponseDTO>.Success(response));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
            return Unauthorized(ApiResponseDTO<object>.Fail("Refresh Token inválido ou expirado", 401));

        return Ok(ApiResponseDTO<AuthResponseDTO>.Success(response));
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var result = await _authService.RevokeTokenAsync(request.Token);

        if (!result)
            return BadRequest(ApiResponseDTO<object>.Fail("Token inválido ou já revogado"));

        return Ok(ApiResponseDTO<object>.Success(null, "Token revogado com sucesso"));
    }

    public record RegisterRequest(string Nome, string Email, string Senha, string Perfil);

    public record LoginRequest(string Email, string Senha);
}
