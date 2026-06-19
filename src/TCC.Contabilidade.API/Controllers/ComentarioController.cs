using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComentarioController : ControllerBase
{
    private readonly ComentarioService _service;

    public ComentarioController(ComentarioService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new Exception("Usuário não identificado.");
        return Guid.Parse(claim.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateComentarioDto dto)
    {
        try
        {
            var result = await _service.AdicionarComentarioAsync(dto, GetUserId());
            return Ok(ApiResponseDTO<ComentarioResponseDto>.Success(result, "Comentário adicionado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpGet("documento/{id}")]
    public async Task<IActionResult> ListarPorDocumento(Guid id)
    {
        try
        {
            var result = await _service.ListarPorDocumentoAsync(id, GetUserId());
            return Ok(ApiResponseDTO<List<ComentarioResponseDto>>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpGet("guia/{id}")]
    public async Task<IActionResult> ListarPorGuia(Guid id)
    {
        try
        {
            var result = await _service.ListarPorGuiaPagamentoAsync(id, GetUserId());
            return Ok(ApiResponseDTO<List<ComentarioResponseDto>>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
