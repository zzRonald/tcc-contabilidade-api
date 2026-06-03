using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Contador,Admin")]
public class AuditoriaController : ControllerBase
{
    private readonly AuditService _auditService;

    public AuditoriaController(AuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Consulta os logs de auditoria da empresa.
    /// </summary>
    /// <param name="filtros">Filtros de consulta e paginação.</param>
    /// <returns>Lista paginada de logs de auditoria.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDTO<IEnumerable<AuditLogResponseDTO>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDTO<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDTO<object>), 403)]
    public async Task<IActionResult> ObterLogs([FromQuery] AuditLogFilterDTO filtros)
    {
        if (filtros.Pagina < 1) filtros.Pagina = 1;
        if (filtros.TamanhoPagina < 1) filtros.TamanhoPagina = 20;
        if (filtros.TamanhoPagina > 100) filtros.TamanhoPagina = 100;

        var (logs, paginacao) = await _auditService.ObterLogsPaginadosAsync(filtros);

        return Ok(ApiResponseDTO<IEnumerable<AuditLogResponseDTO>>.Success(logs, "Logs de auditoria recuperados com sucesso", paginacao));
    }
}
