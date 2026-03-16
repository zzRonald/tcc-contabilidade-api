using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Application.DTO.Empresas;

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
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var empresas = await _service.GetAll(GetUserId());
        return Ok(empresas);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmpresaDto dto)
    {
        await _service.Update(id, dto, GetUserId());
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id, GetUserId());
        return Ok();
    }
}