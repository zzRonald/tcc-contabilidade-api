using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerfilController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuditService _auditService;

    public PerfilController(UserService userService, AuditService auditService)
    {
        _userService = userService;
        _auditService = auditService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Usuário não identificado.");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var userId = GetUserId();
            var user = await _userService.ObterPorIdAsync(userId);

            if (user == null)
                return NotFound(ApiResponseDTO<object>.Fail("Usuário não encontrado"));

            return Ok(ApiResponseDTO<UserResponseDTO>.Success(new UserResponseDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                TipoUsuario = user.TipoUsuario.ToString()
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _userService.UpdateProfileAsync(userId, request.Nome, request.Email);

            await _auditService.RegistrarEvento("Atualização de Perfil", "User", userId.ToString(), userId);

            return Ok(ApiResponseDTO<object>.Success(null, "Perfil atualizado com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPost("alterar-senha")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _userService.ChangePasswordAsync(userId, request.SenhaAtual, request.NovaSenha);

            await _auditService.RegistrarEvento("Alteração de Senha", "User", userId.ToString(), userId);

            return Ok(ApiResponseDTO<object>.Success(null, "Senha alterada com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpGet("exportar-dados")]
    public async Task<IActionResult> ExportarDados()
    {
        try
        {
            var userId = GetUserId();
            var dados = await _userService.ExportarDadosPessoaisAsync(userId);

            await _auditService.RegistrarEvento("Exportação de Dados Pessoais (LGPD)", "User", userId.ToString(), userId);

            return Ok(ApiResponseDTO<object>.Success(dados, "Dados exportados com sucesso em conformidade com a LGPD"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
