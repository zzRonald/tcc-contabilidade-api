using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Services;

namespace TCC.Contabilidade.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ObrigacaoController : ControllerBase
{
    private readonly ObrigacaoService _service;

    public ObrigacaoController(ObrigacaoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ObrigacaoFilterDTO filtros)
    {
        var (itens, paginacao) = await _service.ObterPaginadoAsync(filtros);
        return Ok(ApiResponseDTO<IEnumerable<ObrigacaoResponseDTO>>.Success(itens, paginacao: paginacao));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var result = await _service.ObterPorIdAsync(id);
        if (result == null)
            return NotFound(ApiResponseDTO<object>.Fail("Obrigação não encontrada.", 404));

        return Ok(ApiResponseDTO<ObrigacaoResponseDTO>.Success(result));
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ObrigacaoRequestDTO request)
    {
        try
        {
            var result = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponseDTO<ObrigacaoResponseDTO>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] UpdateObrigacaoRequestDTO request)
    {
        try
        {
            var result = await _service.AtualizarAsync(id, request);
            return Ok(ApiResponseDTO<ObrigacaoResponseDTO>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Contador,Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] UpdateObrigacaoStatusRequestDTO request)
    {
        try
        {
            var result = await _service.AtualizarStatusAsync(id, request);
            return Ok(ApiResponseDTO<ObrigacaoResponseDTO>.Success(result));
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
            return Ok(ApiResponseDTO<object>.Success(null, mensagem: "Obrigação removida com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseDTO<object>.Fail(ex.Message));
        }
    }
}
