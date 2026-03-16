using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IEmpresaRepository
{
    Task AddAsync(Empresa empresa);

    Task<List<Empresa>> GetAllByClienteId(Guid clienteId);

    Task<Empresa?> GetById(Guid id);

    Task<bool> CnpjExists(string cnpj);

    Task Update(Empresa empresa);

    Task Delete(Empresa empresa);
}