using Microsoft.AspNetCore.Mvc;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegracoesController : ControllerBase
{
    private readonly IntegrationService _service;

    public IntegracoesController(IntegrationService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var result = _service.ListarIntegracoes();
        return Ok(ApiResponseDTO<List<ExternalIntegrationDTO>>.Success(result));
    }
}
