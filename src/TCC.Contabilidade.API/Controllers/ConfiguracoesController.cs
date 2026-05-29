using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTOs;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfiguracoesController : ControllerBase
{
    private readonly CompanyConfigService _service;

    public ConfiguracoesController(CompanyConfigService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    private Guid GetCompanyId()
    {
        if (Request.Headers.TryGetValue("X-Company-Id", out var companyIdStr))
        {
            if (Guid.TryParse(companyIdStr, out var companyId))
            {
                return companyId;
            }
        }

        throw new BadHttpRequestException("O cabeçalho X-Company-Id é obrigatório.");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var config = await _service.GetByEmpresaId(GetCompanyId(), GetUserId());
        return Ok(ApiResponseDTO<CompanyConfigDTO>.Success(config, "Configurações obtidas com sucesso"));
    }

    [HttpPut]
    public async Task<IActionResult> Put(CompanyConfigDTO dto)
    {
        var companyId = GetCompanyId();
        if (dto.EmpresaId != companyId)
        {
             return BadRequest(ApiResponseDTO<object>.Fail("ID da empresa no DTO não coincide com o cabeçalho X-Company-Id."));
        }

        await _service.Upsert(dto, GetUserId());
        return Ok(ApiResponseDTO<object>.Success(null!, "Configurações atualizadas com sucesso"));
    }
}
