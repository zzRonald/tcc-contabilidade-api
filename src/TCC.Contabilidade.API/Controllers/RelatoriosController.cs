using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Relatorios;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    [HttpGet("mensal")]
    public async Task<ActionResult<ApiResponseDTO<RelatorioMensalDTO>>> GetRelatorioMensal(
        [FromQuery] Guid empresaId,
        [FromQuery] int mes,
        [FromQuery] int ano)
    {
        var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
        {
            return BadRequest(ApiResponseDTO<RelatorioMensalDTO>.Fail("Usuário não identificado."));
        }

        try
        {
            var relatorio = await _relatorioService.GetRelatorioMensalAsync(empresaId, mes, ano, usuarioId);
            return Ok(ApiResponseDTO<RelatorioMensalDTO>.Success(relatorio));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponseDTO<RelatorioMensalDTO>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<RelatorioMensalDTO>.Fail(ex.Message));
        }
    }
}
