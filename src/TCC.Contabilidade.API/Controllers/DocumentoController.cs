using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentoController : ControllerBase
{
    private readonly DocumentoService _service;

    public DocumentoController(DocumentoService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentoDto dto)
    {
        try
        {
            var result = await _service.UploadAsync(dto, GetUserId());
            return Ok(ApiResponseDTO<DocumentoResponseDto>.Success(result, "Documento enviado com sucesso"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
