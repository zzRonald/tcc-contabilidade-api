using Microsoft.AspNetCore.Http;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class TenantContextService : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var tenantIdStr = _httpContextAccessor.HttpContext?.Items["TenantId"] as string;
            if (Guid.TryParse(tenantIdStr, out var tenantId))
            {
                return tenantId;
            }
            return null;
        }
    }
}
