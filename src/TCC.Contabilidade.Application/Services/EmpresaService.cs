using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.DTO;
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

    public async Task Create(CreateEmpresaDto dto, Guid usuarioId)
    {
        var empresa = await _repository.GetByCnpjAsync(dto.CNPJ);

        if (empresa == null)
        {
            empresa = new Empresa
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                CNPJ = dto.CNPJ
            };
            await _repository.AddAsync(empresa);
        }

        if (!await _repository.IsUsuarioVinculadoAsync(usuarioId, empresa.Id))
        {
            var vinculou = new UsuarioEmpresa
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                EmpresaId = empresa.Id
            };
            await _repository.AddUsuarioEmpresaAsync(vinculou);
        }
    }

    public async Task<(List<EmpresaResponseDto> Items, PaginationMetadataDTO Metadata)> GetAll(Guid usuarioId, int page, int pageSize)
    {
        var (empresas, totalCount) = await _repository.GetPagedByUsuarioId(usuarioId, page, pageSize);

        var items = empresas.Select(e => new EmpresaResponseDto
        {
            Id = e.Id,
            NomeFantasia = e.Nome,
            CNPJ = e.CNPJ
        }).ToList();

        var metadata = new PaginationMetadataDTO
        {
            PaginaAtual = page,
            TamanhoPagina = pageSize,
            TotalRegistros = totalCount,
            TotalPaginas = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return (items, metadata);
    }

    public async Task Update(Guid id, UpdateEmpresaDto dto, Guid usuarioId)
    {
        if (!await _repository.IsUsuarioVinculadoAsync(usuarioId, id))
            throw new UnauthorizedAccessException("Usuário não tem permissão para editar esta empresa.");

        var empresaComMesmoCnpj = await _repository.GetByCnpjAsync(dto.CNPJ);
        if (empresaComMesmoCnpj != null && empresaComMesmoCnpj.Id != id)
            throw new Exception("Já existe outra empresa cadastrada com este CNPJ.");

        var empresa = await _repository.GetById(id);

        if (empresa == null)
            throw new Exception("Empresa não encontrada");

        empresa.Nome = dto.Nome;
        empresa.CNPJ = dto.CNPJ;

        await _repository.Update(empresa);
    }

    public async Task Delete(Guid id, Guid usuarioId)
    {
        if (!await _repository.IsUsuarioVinculadoAsync(usuarioId, id))
            throw new UnauthorizedAccessException("Usuário não tem permissão para excluir esta empresa.");

        var empresa = await _repository.GetById(id);

        if (empresa == null)
            throw new Exception("Empresa não encontrada");

        await _repository.Delete(empresa);
    }
}
