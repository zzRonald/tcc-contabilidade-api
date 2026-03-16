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

    public async Task<List<Empresa>> GetAllByClienteId(Guid clienteId)
    {
        return await _context.Empresas
            .AsNoTracking()
            .Where(e => e.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<Empresa?> GetById(Guid id)
    {
        return await _context.Empresas.FirstOrDefaultAsync(e => e.Id == id);
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
}