using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly AppDbContext _context;

    public EmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Empresa empresa)
    {
        await _context.Empresas.AddAsync(empresa);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Empresa>> GetAllByUsuarioId(Guid usuarioId)
    {
        return await _context.UsuariosEmpresas
            .AsNoTracking()
            .Where(ue => ue.UsuarioId == usuarioId)
            .Select(ue => ue.Empresa!)
            .ToListAsync();
    }

    public async Task<(List<Empresa> Items, int TotalCount)> GetPagedByUsuarioId(Guid usuarioId, int page, int pageSize)
    {
        var query = _context.UsuariosEmpresas
            .AsNoTracking()
            .Where(ue => ue.UsuarioId == usuarioId)
            .Select(ue => ue.Empresa!);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Empresa?> GetById(Guid id)
    {
        return await _context.Empresas.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empresa?> GetByCnpjAsync(string cnpj)
    {
        return await _context.Empresas.FirstOrDefaultAsync(e => e.CNPJ == cnpj);
    }

    public async Task<bool> CnpjExists(string cnpj)
    {
        return await _context.Empresas.AnyAsync(e => e.CNPJ == cnpj);
    }

    public async Task Update(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Empresa empresa)
    {
        _context.Empresas.Remove(empresa);
        await _context.SaveChangesAsync();
    }

    public async Task AddUsuarioEmpresaAsync(UsuarioEmpresa usuarioEmpresa)
    {
        await _context.UsuariosEmpresas.AddAsync(usuarioEmpresa);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUsuarioVinculadoAsync(Guid usuarioId, Guid empresaId)
    {
        return await _context.UsuariosEmpresas.AnyAsync(ue => ue.UsuarioId == usuarioId && ue.EmpresaId == empresaId);
    }

    public async Task<int> CountByUsuarioId(Guid usuarioId)
    {
        return await _context.UsuariosEmpresas
            .Where(ue => ue.UsuarioId == usuarioId)
            .CountAsync();
    }
}
