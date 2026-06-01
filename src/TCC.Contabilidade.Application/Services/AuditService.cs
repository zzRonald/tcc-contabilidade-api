using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class AuditService
{
    private readonly IAuditRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IAuditRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
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
            Ip = ip
        };

        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();
    }
}
