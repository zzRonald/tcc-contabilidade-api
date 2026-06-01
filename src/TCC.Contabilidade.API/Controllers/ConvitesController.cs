using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Convites;
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

    [HttpGet]
    public async Task<IActionResult> ListarConvites([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized(ApiResponseDTO<object>.Fail("Não foi possível identificar o usuário autenticado.", 401));
        }

        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResponseDTO<object>.Fail("Os parâmetros de paginação devem ser maiores que zero."));
        }

        if (pageSize > 100)
        {
            return BadRequest(ApiResponseDTO<object>.Fail("O tamanho máximo da página é 100."));
        }

        var contadorId = Guid.Parse(userIdClaim.Value);

        var (items, metadata) = await _conviteService.GetPagedConvitesByContadorIdAsync(contadorId, page, pageSize);

        return Ok(ApiResponseDTO<IEnumerable<ConviteResponseDto>>.Success(items, "Convites listados com sucesso", metadata));
    }

    public record CriarConviteRequest(string EmailCliente);
}