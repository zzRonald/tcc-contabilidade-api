using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface ICompanyConfigRepository
{
    Task<CompanyConfig?> GetByEmpresaIdAsync(Guid empresaId);
    Task AddAsync(CompanyConfig config);
    Task UpdateAsync(CompanyConfig config);
}
