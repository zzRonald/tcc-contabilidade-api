using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Infrastructure.Repositories;

public class CompanyConfigRepository : ICompanyConfigRepository
{
    private readonly AppDbContext _context;

    public CompanyConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyConfig?> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await _context.CompanyConfigs
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);
    }

    public async Task AddAsync(CompanyConfig config)
    {
        await _context.CompanyConfigs.AddAsync(config);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CompanyConfig config)
    {
        _context.CompanyConfigs.Update(config);
        await _context.SaveChangesAsync();
    }
}
