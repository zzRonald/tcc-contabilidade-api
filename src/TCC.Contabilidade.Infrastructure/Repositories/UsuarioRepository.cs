using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<User?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AdicionarAsync(User usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task AtualizarAsync(User usuario)
    {
        _context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountClientesByContadorId(Guid contadorId)
    {
        return await _context.Usuarios
            .Where(u => u.ContadorId == contadorId)
            .CountAsync();
    }
}