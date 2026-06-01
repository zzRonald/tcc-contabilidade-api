using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TCC.Contabilidade.API.Middlewares;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // O middleware pode ser expandido futuramente para capturar logs genéricos de requisição.
        // Por enquanto, as ações específicas são logadas nos serviços.
        await _next(context);
    }
}
