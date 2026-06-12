using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompetenciasController : ControllerBase
{
    private readonly CompetenciaService _service;

    public CompetenciasController(CompetenciaService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCompetenciaDto dto)
    {
        try
        {
            await _service.Create(dto, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null!, "Competência criada com sucesso"));
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
            var (items, metadata) = await _service.GetAll(empresaId, GetUserId(), page, pageSize);
            return Ok(ApiResponseDTO<IEnumerable<CompetenciaResponseDto>>.Success(items, "Competências listadas com sucesso", metadata));
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
            var competencia = await _service.GetById(id, GetUserId());
            return Ok(ApiResponseDTO<CompetenciaResponseDto>.Success(competencia));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateCompetenciaStatusDto dto)
    {
        try
        {
            await _service.UpdateStatus(id, dto, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null!, "Status da competência atualizado com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
