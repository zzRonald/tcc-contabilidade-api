using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _config;

    public AuthController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var usuario = await _userService.RegisterAsync(request.Nome, request.Email, request.Senha, request.Perfil);
            return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _userService.AuthenticateAsync(request.Email, request.Senha);
        if (usuario == null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        var token = GenerateJwtToken(usuario);
        return Ok(new { token });
    }

    // Gera JWT
    private string GenerateJwtToken(Usuario usuario)
    {
        var keyBytes = Encoding.ASCII.GetBytes(_config["JwtKey"] ?? "MinhaChaveSuperSecretaParaJWT_ChangeThis");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            }),
            Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Models de requisição
    public record RegisterRequest(string Nome, string Email, string Senha, string Perfil);
    public record LoginRequest(string Email, string Senha);
}