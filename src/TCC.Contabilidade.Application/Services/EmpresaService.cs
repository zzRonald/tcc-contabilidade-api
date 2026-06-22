using TCC.Contabilidade.Application.DTO.Empresas;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Utils;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class EmpresaService
{
    private readonly IEmpresaRepository _repository;
    private readonly AuditService _auditService;
    private readonly CacheService _cache;
    private const string CachePrefix = "empresas";

    public EmpresaService(IEmpresaRepository repository, AuditService auditService, CacheService cache)
    {
        _repository = repository;
        _auditService = auditService;
        _cache = cache;
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

        await _auditService.RegistrarEvento("CREATE_EMPRESA", "Empresa", empresa.Id.ToString(), usuarioId);
        await InvalidateUserCache(usuarioId);
    }

    public async Task<(List<EmpresaResponseDto> Items, PaginationMetadataDTO Metadata)> GetAll(Guid usuarioId, int page, int pageSize)
    {
        string versionKey = _cache.GenerateKey(CachePrefix, usuarioId, "version");
        string version = await _cache.GetOrCreateAsync(versionKey, () => Task.FromResult(Guid.NewGuid().ToString()), TimeSpan.FromHours(1)) ?? "1";

        string cacheKey = _cache.GenerateKey(CachePrefix, usuarioId, version, page, pageSize);

        var cached = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var (empresas, totalCount) = await _repository.GetPagedByUsuarioId(usuarioId, page, pageSize);

            var items = empresas.Select(e => new EmpresaResponseDto
            {
                Id = e.Id,
                NomeFantasia = e.Nome,
                CNPJ = PrivacyUtils.MaskCnpj(e.CNPJ)
            }).ToList();

            var metadata = new PaginationMetadataDTO
            {
                PaginaAtual = page,
                TamanhoPagina = pageSize,
                TotalRegistros = totalCount,
                TotalPaginas = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new EmpresaListResponse { Items = items, Metadata = metadata };
        }, TimeSpan.FromMinutes(5));

        return (cached?.Items ?? new List<EmpresaResponseDto>(), cached?.Metadata ?? new PaginationMetadataDTO());
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

        await _auditService.RegistrarEvento("UPDATE_EMPRESA", "Empresa", id.ToString(), usuarioId);
        await InvalidateUserCache(usuarioId);
    }

    public async Task Delete(Guid id, Guid usuarioId)
    {
        if (!await _repository.IsUsuarioVinculadoAsync(usuarioId, id))
            throw new UnauthorizedAccessException("Usuário não tem permissão para excluir esta empresa.");

        var empresa = await _repository.GetById(id);

        if (empresa == null)
            throw new Exception("Empresa não encontrada");

        await _repository.Delete(empresa);

        await _auditService.RegistrarEvento("EXCLUSAO_EMPRESA", "Empresa", id.ToString(), usuarioId);
        await InvalidateUserCache(usuarioId);
    }

    private async Task InvalidateUserCache(Guid usuarioId)
    {
        // Invalida a versão do cache para o usuário, forçando novas consultas
        await _cache.RemoveAsync(_cache.GenerateKey(CachePrefix, usuarioId, "version"));
    }

    private class EmpresaListResponse
    {
        public List<EmpresaResponseDto> Items { get; set; } = new();
        public PaginationMetadataDTO Metadata { get; set; } = new();
    }
}
