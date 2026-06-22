using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Contador")]
public class UsuariosController : ControllerBase
{
    private readonly UserService _userService;

    public UsuariosController(UserService userService)
    {
        _userService = userService;
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

    private TipoUsuario GetUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if (Enum.TryParse<TipoUsuario>(roleClaim, true, out var role))
        {
            return role;
        }
        throw new UnauthorizedAccessException("Perfil de usuário não identificado.");
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UserFilterDTO filtros)
    {
        try
        {
            var (usuarios, paginacao) = await _userService.ObterUsuariosPaginadosAsync(
                filtros,
                GetUserId(),
                GetUserRole()
            );

            return Ok(ApiResponseDTO<IEnumerable<UserAdminResponseDTO>>.Success(usuarios, paginacao: paginacao));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _userService.ObterPorIdAsync(id);

            if (user == null)
                return NotFound(ApiResponseDTO<object>.Fail("Usuário não encontrado"));

            var currentUserId = GetUserId();
            var currentUserRole = GetUserRole();

            // Validação de acesso
            if (currentUserRole == TipoUsuario.Contador && user.ContadorId != currentUserId)
            {
                return Forbid();
            }

            return Ok(ApiResponseDTO<UserAdminResponseDTO>.Success(new UserAdminResponseDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                TipoUsuario = user.TipoUsuario.ToString(),
                Ativo = user.Ativo
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
    {
        try
        {
            await _userService.UpdateStatusAsync(id, request.Ativo, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null, "Status do usuário atualizado com sucesso"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/perfil")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            await _userService.UpdateRoleAsync(id, request.NovoPerfil, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null, "Perfil do usuário atualizado com sucesso"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
