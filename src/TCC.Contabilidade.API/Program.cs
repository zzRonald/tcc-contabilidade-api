using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =============================
// JWT CONFIG
// =============================

var key = Encoding.ASCII.GetBytes("MinhaChaveSuperSecretaParaJWT_ChangeThis");

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

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ConviteService>();

// =============================
// CONTROLLERS
// =============================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANTE: ordem correta
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();