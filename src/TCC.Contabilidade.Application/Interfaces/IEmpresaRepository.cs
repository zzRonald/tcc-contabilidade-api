using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IEmpresaRepository
{
    Task AddAsync(Empresa empresa);

    Task<List<Empresa>> GetAllByUsuarioId(Guid usuarioId);

    Task<Empresa?> GetById(Guid id);

    Task<Empresa?> GetByCnpjAsync(string cnpj);

    Task<bool> CnpjExists(string cnpj);

    Task Update(Empresa empresa);

    Task Delete(Empresa empresa);

    Task AddUsuarioEmpresaAsync(UsuarioEmpresa usuarioEmpresa);

    Task<bool> IsUsuarioVinculadoAsync(Guid usuarioId, Guid empresaId);
}
