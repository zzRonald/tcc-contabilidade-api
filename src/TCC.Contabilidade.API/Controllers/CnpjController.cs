using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

            return Ok(new
            {
                sucesso = true,
                dados = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { sucesso = false, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    // 🔐 Método para pegar tipo do usuário (JWT futuramente)
    private TipoUsuario ObterTipoUsuario()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "tipoUsuario");

        // 🔥 TEMPORÁRIO (até implementar JWT)
        if (claim == null)
            return TipoUsuario.Contador;

        return (TipoUsuario)int.Parse(claim.Value);
    }
}