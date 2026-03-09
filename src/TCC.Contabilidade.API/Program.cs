using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =============================
// 1. Configuração do Banco de Dados
// =============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TCC.Contabilidade.Infrastructure")
    ));

// =============================
// 2. Injeção de Dependência (Sua correção principal aqui!)
// =============================
// Interface -> Implementação
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IConviteRepository, ConviteRepository>();
builder.Services.AddScoped<ConviteService>();

// Serviços da Camada de Application
builder.Services.AddScoped<UserService>();

// =============================
// 3. Controllers e Swagger
// =============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>

{
    c.SwaggerDoc("v1", new() { Title = "TCC Contabilidade API", Version = "v1" });
});

var app = builder.Build();

// =============================
// 4. Configure Middleware
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: Authentication vem SEMPRE antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();