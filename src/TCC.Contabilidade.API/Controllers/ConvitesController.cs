using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Contador")]
public class ConvitesController : ControllerBase
{
    private readonly ConviteService _conviteService;

    public ConvitesController(ConviteService conviteService)
    {
        _conviteService = conviteService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarConvite([FromBody] CriarConviteRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                mensagem = "Não foi possível identificar o usuário autenticado."
            });
        }

        var contadorId = Guid.Parse(userIdClaim.Value);

        var token = await _conviteService.CriarConviteAsync(request.EmailCliente, contadorId);

        return Ok(new
        {
            mensagem = "Convite criado com sucesso. O cliente poderá utilizar o token para realizar o cadastro no sistema.",
            token
        });
    }

    public record CriarConviteRequest(string EmailCliente);
}