using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Contador,Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<ApiResponseDTO<DashboardSummaryDTO>>> GetResumo()
    {
        var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
        {
            return BadRequest(ApiResponseDTO<DashboardSummaryDTO>.Fail("Usuário não identificado."));
        }

        var resumo = await _dashboardService.GetResumoAsync(usuarioId);

        return Ok(ApiResponseDTO<DashboardSummaryDTO>.Success(resumo));
    }
}
