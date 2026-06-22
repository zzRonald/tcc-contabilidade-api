using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
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

    public async Task<List<User>> ObterUsuariosPorEmpresaAsync(Guid empresaId)
    {
        return await _context.UsuariosEmpresas
            .IgnoreQueryFilters()
            .Where(ue => ue.EmpresaId == empresaId)
            .Select(ue => ue.Usuario!)
            .ToListAsync();
    }

    public async Task<(IEnumerable<User> Usuarios, int Total)> ObterUsuariosPaginadosAsync(
        string? nome = null,
        string? email = null,
        string? tipoUsuario = null,
        bool? ativo = null,
        Guid? contadorId = null,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        var query = _context.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(u => u.Nome.Contains(nome));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));

        if (!string.IsNullOrWhiteSpace(tipoUsuario) && Enum.TryParse<TipoUsuario>(tipoUsuario, true, out var tipo))
            query = query.Where(u => u.TipoUsuario == tipo);

        if (ativo.HasValue)
            query = query.Where(u => u.Ativo == ativo.Value);

        if (contadorId.HasValue)
            query = query.Where(u => u.ContadorId == contadorId.Value);

        var total = await query.CountAsync();

        var usuarios = await query
            .OrderBy(u => u.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (usuarios, total);
    }
}