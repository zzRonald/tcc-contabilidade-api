using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class AuditService
{
    private readonly IAuditRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenantContext;

    public AuditService(IAuditRepository repository, IHttpContextAccessor httpContextAccessor, ITenantContext tenantContext)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _tenantContext = tenantContext;
    }

    public async Task RegistrarEvento(string acao, string entidade, string? entidadeId = null, Guid? usuarioId = null)
    {
        var context = _httpContextAccessor.HttpContext;
        var ip = context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        if (usuarioId == null)
        {
            var userIdClaim = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var parsedId))
            {
                usuarioId = parsedId;
            }
        }

        var log = new AuditLog
        {
            UsuarioId = usuarioId,
            Acao = acao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            DataHora = DateTime.UtcNow,
            Ip = ip,
            EmpresaId = _tenantContext.TenantId ?? Guid.Empty
        };

        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();
    }

    public async Task<(IEnumerable<AuditLogResponseDTO> Logs, PaginationMetadataDTO Paginacao)> ObterLogsPaginadosAsync(AuditLogFilterDTO filtros)
    {
        var (logs, total) = await _repository.GetPagedAsync(filtros);

        var paginacao = new PaginationMetadataDTO
        {
            PaginaAtual = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalRegistros = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };

        return (logs, paginacao);
    }
}
