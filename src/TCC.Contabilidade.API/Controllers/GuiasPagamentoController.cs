using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GuiasPagamentoController : ControllerBase
{
    private readonly GuiaPagamentoService _service;

    public GuiasPagamentoController(GuiaPagamentoService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] GuiaPagamentoFilterDTO filtros)
    {
        var (itens, paginacao) = await _service.ObterPaginadoAsync(filtros);
        return Ok(ApiResponseDTO<IEnumerable<GuiaPagamentoResponseDTO>>.Success(itens, paginacao: paginacao));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var result = await _service.ObterPorIdAsync(id);
        if (result == null)
            return NotFound(ApiResponseDTO<object>.Fail("Guia de pagamento não encontrada.", 404));

        return Ok(ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result));
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] GuiaPagamentoRequestDTO request)
    {
        try
        {
            var result = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize]
    [HttpPost("{id}/comprovante")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EnviarComprovante(Guid id, Microsoft.AspNetCore.Http.IFormFile arquivo)
    {
        try
        {
            var result = await _service.EnviarComprovanteAsync(id, arquivo, GetUserId());
            return Ok(ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result, "Comprovante enviado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPost("{id}/confirmar-pagamento")]
    public async Task<IActionResult> ConfirmarPagamento(Guid id)
    {
        try
        {
            var result = await _service.ConfirmarPagamentoAsync(id, GetUserId());
            return Ok(ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result, "Pagamento confirmado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] UpdateGuiaPagamentoRequestDTO request)
    {
        try
        {
            var result = await _service.AtualizarAsync(id, request);
            return Ok(ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] UpdateGuiaPagamentoStatusRequestDTO request)
    {
        try
        {
            var result = await _service.AtualizarStatusAsync(id, request);
            return Ok(ApiResponseDTO<GuiaPagamentoResponseDTO>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            await _service.RemoverAsync(id);
            return Ok(ApiResponseDTO<object>.Success(null, mensagem: "Guia de pagamento removida com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
