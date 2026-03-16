using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class EmpresaService
{
    private readonly IEmpresaRepository _repository;

    public EmpresaService(IEmpresaRepository repository)
    {
        _repository = repository;
    }

    public async Task Create(CreateEmpresaDto dto, Guid clienteId)
    {
        if (await _repository.CnpjExists(dto.CNPJ))
            throw new Exception("CNPJ já cadastrado.");

        var empresa = new Empresa
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            CNPJ = dto.CNPJ,
            ClienteId = clienteId
        };

        await _repository.AddAsync(empresa);
    }

    public async Task<List<EmpresaResponseDto>> GetAll(Guid clienteId)
    {
        var empresas = await _repository.GetAllByClienteId(clienteId);

        return empresas.Select(e => new EmpresaResponseDto
        {
            Id = e.Id,
            Nome = e.Nome,
            CNPJ = e.CNPJ
        }).ToList();
    }

    public async Task Update(Guid id, UpdateEmpresaDto dto, Guid clienteId)
    {
        var empresa = await _repository.GetById(id);

        if (empresa == null)
            throw new Exception("Empresa não encontrada");

        if (empresa.ClienteId != clienteId)
            throw new UnauthorizedAccessException();

        empresa.Nome = dto.Nome;
        empresa.CNPJ = dto.CNPJ;

        await _repository.Update(empresa);
    }

    public async Task Delete(Guid id, Guid clienteId)
    {
        var empresa = await _repository.GetById(id);

        if (empresa == null)
            throw new Exception("Empresa não encontrada");

        if (empresa.ClienteId != clienteId)
            throw new UnauthorizedAccessException();

        await _repository.Delete(empresa);
    }
}