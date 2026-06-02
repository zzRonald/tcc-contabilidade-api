using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TCC.Contabilidade.API.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // X-Frame-Options: Evita clickjacking ao proibir que a página seja exibida em frames/iframes
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // X-Content-Type-Options: Evita que o navegador tente adivinhar o tipo de conteúdo (MIME sniffing)
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Referrer-Policy: Controla quais informações de referência são enviadas em requisições
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // X-XSS-Protection: Ativa o filtro de Cross-site Scripting (XSS) nos navegadores
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Content-Security-Policy: Controla quais recursos podem ser carregados no navegador
            // Configuração que permite o funcionamento do Swagger (necessita de unsafe-inline para o UI)
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self';";

            await _next(context);
        }
    }
}
