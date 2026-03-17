using System.Security.Claims;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.API.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitService _rateLimitService;

        public RateLimitMiddleware(RequestDelegate next, IRateLimitService rateLimitService)
        {
            _next = next;
            _rateLimitService = rateLimitService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            var method = context.Request.Method;

            var (limit, window) = GetLimit(path, method);

            if (limit == 0)
            {
                await _next(context);
                return;
            }

            var clientId = GetClientIdentifier(context);

            var key = $"{clientId}:{method}:{path}";

            var allowed = await _rateLimitService.IsRequestAllowedAsync(key, limit, window);

            if (!allowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                await context.Response.WriteAsJsonAsync(new
                {
                    erro = "Muitas requisições",
                    mensagem = "Limite de requisições excedido. Tente novamente mais tarde."
                });

                return;
            }

            await _next(context);
        }

        private (int limit, TimeSpan window) GetLimit(string path, string method)
        {
            if (path == "/api/auth/login" && method == "POST")
                return (5, TimeSpan.FromMinutes(1));

            if (path == "/api/auth/register" && method == "POST")
                return (5, TimeSpan.FromMinutes(1));

            if (path == "/api/convites" && method == "POST")
                return (10, TimeSpan.FromMinutes(1));

            if (path == "/api/empresas" && method == "GET")
                return (60, TimeSpan.FromMinutes(1));

            return (0, TimeSpan.Zero);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            return $"{ip}:{userId}";
        }
    }
}