using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Application.DTOs;
using TCC.Contabilidade.Application.DTOs.Convites;

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

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new Exception("Não foi possível identificar o usuário autenticado.");

        return Guid.Parse(userIdClaim.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CriarConvite([FromBody] CreateConviteDto dto)
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            return BadRequest(ApiResponseDTO<IEnumerable<string>>.Fail(
                string.Join(" | ", erros),
                400
            ));
        }

        var contadorId = GetUserId();

        var token = await _conviteService.CriarConviteAsync(dto.EmailDestino, contadorId);

        return Ok(ApiResponseDTO<object>.Success(
            new { token },
            "Convite criado com sucesso. O cliente poderá utilizar o token para realizar o cadastro."
        ));
    }

    [HttpGet]
    public async Task<IActionResult> ListarConvites()
    {
        var contadorId = GetUserId();

        var convites = await _conviteService.GetConvitesByContadorIdAsync(contadorId);

        return Ok(ApiResponseDTO<object>.Success(
            convites,
            "Convites listados com sucesso."
        ));
    }
}