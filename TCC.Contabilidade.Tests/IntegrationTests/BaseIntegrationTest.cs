using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Tests.Fixtures;

namespace TCC.Contabilidade.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<(string Token, User User)> AuthenticateAsync(TipoUsuario tipo = TipoUsuario.Admin, Guid? tenantId = null)
    {
        var email = $"{tipo.ToString().ToLower()}_{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(password);
        User user;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            user = new User(tipo.ToString() + " User", email, senhaHash, tipo);
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
        }

        string token;
        using (var scope = Factory.Services.CreateScope())
        {
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbUser = await db.Usuarios.FindAsync(user.Id);
            token = tokenService.GenerateAccessToken(dbUser!, tenantId);
        }

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return (token, user);
    }

    protected async Task<Empresa> CreateEmpresaAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var empresa = new Empresa
        {
            Nome = "Empresa " + Guid.NewGuid().ToString().Substring(0, 8),
            CNPJ = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 14)
        };

        db.Empresas.Add(empresa);
        await db.SaveChangesAsync();
        return empresa;
    }

    protected async Task VincularUsuarioEmpresaAsync(Guid usuarioId, Guid empresaId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.UsuariosEmpresas.Add(new UsuarioEmpresa { UsuarioId = usuarioId, EmpresaId = empresaId });
        await db.SaveChangesAsync();
    }
}
