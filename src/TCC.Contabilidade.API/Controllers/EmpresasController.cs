using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresasController : ControllerBase
{
    private readonly EmpresaService _service;

    public EmpresasController(EmpresaService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmpresaDto dto)
    {
        await _service.Create(dto, GetUserId());

        return Ok(ApiResponseDTO<object>
            .Success(null!, "Empresa criada com sucesso"));
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResponseDTO<object>.Fail("Os parâmetros de paginação devem ser maiores que zero."));
        }

        if (pageSize > 100)
        {
            return BadRequest(ApiResponseDTO<object>.Fail("O tamanho máximo da página é 100."));
        }

        var (items, metadata) = await _service.GetAll(GetUserId(), page, pageSize);

        return Ok(ApiResponseDTO<IEnumerable<EmpresaResponseDto>>
            .Success(items, "Empresas listadas com sucesso", metadata));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var (items, _) = await _service.GetAll(GetUserId(), 1, 1000);
        var empresa = items.FirstOrDefault(e => e.Id == id);

        if (empresa == null)
            return NotFound(ApiResponseDTO<object>.Fail("Empresa não encontrada"));

        return Ok(ApiResponseDTO<EmpresaResponseDto>.Success(empresa));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmpresaDto dto)
    {
        await _service.Update(id, dto, GetUserId());

        return Ok(ApiResponseDTO<object>
            .Success(null!, "Empresa atualizada com sucesso"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id, GetUserId());

        return Ok(ApiResponseDTO<object>
            .Success(null!, "Empresa removida com sucesso"));
    }
}