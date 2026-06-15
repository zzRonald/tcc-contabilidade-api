using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitacaoDocumentoController : ControllerBase
{
    private readonly SolicitacaoDocumentoService _service;

    public SolicitacaoDocumentoController(SolicitacaoDocumentoService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSolicitacaoDocumentoDto dto)
    {
        try
        {
            await _service.Create(dto, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null!, "Solicitação de documento criada com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpGet("empresa/{empresaId}")]
    public async Task<IActionResult> GetByEmpresa(Guid empresaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var (items, metadata) = await _service.GetByEmpresa(empresaId, GetUserId(), page, pageSize);
            return Ok(ApiResponseDTO<IEnumerable<SolicitacaoDocumentoResponseDto>>.Success(items, "Solicitações listadas com sucesso", metadata));
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
            var solicitacao = await _service.GetById(id, GetUserId());
            return Ok(ApiResponseDTO<SolicitacaoDocumentoResponseDto>.Success(solicitacao));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateSolicitacaoStatusDto dto)
    {
        try
        {
            await _service.UpdateStatus(id, dto, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null!, "Status da solicitação atualizado com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
