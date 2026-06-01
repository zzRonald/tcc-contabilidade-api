using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace TCC.Contabilidade.API.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = context.User.Claims
            .FirstOrDefault(c => c.Type == "tenantId")?.Value;

        if (!string.IsNullOrEmpty(tenantId))
        {
            context.Items["TenantId"] = tenantId;
        }

        await _next(context);
    }
}
