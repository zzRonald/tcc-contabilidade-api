using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TCC.Contabilidade.API.Middlewares;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Infrastructure.Cache;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Infrastructure.Integrations;
using TCC.Contabilidade.Infrastructure.Repositories;
using TCC.Contabilidade.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// =============================
// JWT CONFIG
// =============================

var key = Encoding.ASCII.GetBytes(
    builder.Configuration["Jwt:Key"] ?? "MinhaChaveSuperSecretaParaJWT_ChangeThis"
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                mensagem = "Seu nível de acesso não permite esta operação."
            });

            await context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();


// =============================
// DATABASE
// =============================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TCC.Contabilidade.Infrastructure")
    ));


// =============================
// DEPENDENCY INJECTION
// =============================

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IConviteRepository, ConviteRepository>();
builder.Services.AddScoped<ICompanyConfigRepository, CompanyConfigRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ConviteService>();
builder.Services.AddScoped<CompanyConfigService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

//  ADICIONAR PARA EMPRESAS
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<ICompetenciaRepository, CompetenciaRepository>();
builder.Services.AddScoped<CompetenciaService>();

// INTEGRAÇÕES
builder.Services.AddHttpClient<CnpjIntegrationService>();
builder.Services.AddScoped<ICnpjApiClient>(sp => sp.GetRequiredService<CnpjIntegrationService>());
builder.Services.AddScoped<IExternalIntegration>(sp => sp.GetRequiredService<CnpjIntegrationService>());
builder.Services.AddScoped<CnpjService>();
builder.Services.AddScoped<IntegrationService>();

// TENANT CONTEXT
builder.Services.AddScoped<ITenantContext, TenantContextService>();

// CACHE
builder.Services.AddSingleton<ICacheService, MemoryCacheProvider>();
builder.Services.AddSingleton<CacheService>();


// =============================
// CONTROLLERS
// =============================

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();

// =============================
// CORS CONFIG
// =============================

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});


// =============================
// SWAGGER + JWT
// =============================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TCC Contabilidade API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu token JWT}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();


// =============================
// MIDDLEWARE
// =============================

app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<AuditMiddleware>();
app.UseAuthorization();

//  RATE LIMITING 
app.UseMiddleware<TCC.Contabilidade.API.Middlewares.RateLimitMiddleware>();

app.MapControllers();

app.Run();
