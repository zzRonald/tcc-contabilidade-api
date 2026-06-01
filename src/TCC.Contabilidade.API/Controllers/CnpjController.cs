using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CnpjController : ControllerBase
{
    private readonly CnpjService _service;

    public CnpjController(CnpjService service)
    {
        _service = service;
    }

    [HttpGet("{cnpj}")]
    public async Task<IActionResult> Consultar(string cnpj)
    {
        try
        {
            var tipoUsuario = ObterTipoUsuario();

            var result = await _service.ConsultarCnpj(cnpj, tipoUsuario);

            return Ok(ApiResponseDTO<CnpjResponseDTO>.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseDTO<object>.Fail(ex.Message, 401));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    // 🔐 Método para pegar tipo do usuário
    private TipoUsuario ObterTipoUsuario()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "tipoUsuario")
                    ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        if (claim == null)
            return TipoUsuario.Contador;

        if (Enum.TryParse<TipoUsuario>(claim.Value, out var tipo))
            return tipo;

        return TipoUsuario.Contador;
    }
}