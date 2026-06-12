using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface ICompetenciaRepository
{
    Task<Competencia?> GetByIdAsync(Guid id);
    Task<Competencia?> GetByMesAnoAsync(Guid empresaId, int mes, int ano);
    Task<(List<Competencia> Items, int TotalCount)> GetPagedByEmpresaIdAsync(Guid empresaId, int page, int pageSize);
    Task AddAsync(Competencia competencia);
    Task UpdateAsync(Competencia competencia);
    Task<bool> ExistsAsync(Guid empresaId, int mes, int ano);
}
