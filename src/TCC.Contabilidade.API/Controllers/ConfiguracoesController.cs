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
        var headerValue = Request.Headers["X-Company-Id"].ToString();
        if (string.IsNullOrEmpty(headerValue))
            throw new Exception("O header X-Company-Id é obrigatório para esta operação.");

        return Guid.Parse(headerValue);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var config = await _service.GetByEmpresaId(GetCompanyId(), GetUserId());
            return Ok(ApiResponseDTO<CompanyConfigDto>.Success(config, "Configurações obtidas com sucesso"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseDTO<object>.Fail(ex.Message, 401));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message, 400));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(CompanyConfigDto dto)
    {
        try
        {
            await _service.Update(GetCompanyId(), dto, GetUserId());
            return Ok(ApiResponseDTO<object>.Success(null!, "Configurações atualizadas com sucesso"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseDTO<object>.Fail(ex.Message, 401));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message, 400));
        }
    }
}
