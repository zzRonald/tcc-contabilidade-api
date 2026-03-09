using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConvitesController : ControllerBase
{
    private readonly ConviteService _conviteService;

    public ConvitesController(ConviteService conviteService)
    {
        _conviteService = conviteService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CriarConvite([FromBody] CriarConviteRequest request)
    {
        var contadorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var token = await _conviteService.CriarConviteAsync(request.EmailCliente, contadorId);

        return Ok(new
        {
            mensagem = "Convite criado com sucesso",
            token
        });
    }

    public record CriarConviteRequest(string EmailCliente);
}