using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Competencias;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class CompetenciaService
{
    private readonly ICompetenciaRepository _competenciaRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AuditService _auditService;

    public CompetenciaService(
        ICompetenciaRepository competenciaRepository,
        IEmpresaRepository empresaRepository,
        IUsuarioRepository usuarioRepository,
        AuditService auditService)
    {
        _competenciaRepository = competenciaRepository;
        _empresaRepository = empresaRepository;
        _usuarioRepository = usuarioRepository;
        _auditService = auditService;
    }

    public async Task Create(CreateCompetenciaDto dto, Guid usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuário não encontrado");

        // Apenas contador ou admin podem criar competências
        if (usuario.TipoUsuario != TipoUsuario.Contador && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Sem permissão para criar competências");

        // Validar vínculo com a empresa (se for contador, precisa estar vinculado ou ser o contador da empresa - simplificado por IsUsuarioVinculadoAsync)
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, dto.EmpresaId);
        if (!vinculado && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Usuário não vinculado a esta empresa");

        if (dto.Mes < 1 || dto.Mes > 12) throw new Exception("Mês inválido");
        if (dto.Ano < 2000 || dto.Ano > 2100) throw new Exception("Ano inválido");

        if (await _competenciaRepository.ExistsAsync(dto.EmpresaId, dto.Mes, dto.Ano))
            throw new Exception("Já existe uma competência cadastrada para este mês/ano nesta empresa");

        var competencia = new Competencia
        {
            EmpresaId = dto.EmpresaId,
            Mes = dto.Mes,
            Ano = dto.Ano,
            Observacoes = dto.Observacoes
        };

        await _competenciaRepository.AddAsync(competencia);
        await _auditService.RegistrarEvento("CREATE_COMPETENCIA", "Competencia", competencia.Id.ToString(), usuarioId);
    }

    public async Task<(IEnumerable<CompetenciaResponseDto> Items, PaginationMetadataDTO Metadata)> GetAll(Guid empresaId, Guid usuarioId, int page, int pageSize)
    {
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId);
        if (!vinculado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario?.TipoUsuario != TipoUsuario.Admin)
                throw new Exception("Usuário não tem acesso a esta empresa");
        }

        var (items, totalCount) = await _competenciaRepository.GetPagedByEmpresaIdAsync(empresaId, page, pageSize);

        var dtos = items.Select(MapToDto);

        var metadata = new PaginationMetadataDTO
        {
            PaginaAtual = page,
            TamanhoPagina = pageSize,
            TotalRegistros = totalCount,
            TotalPaginas = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return (dtos, metadata);
    }

    public async Task UpdateStatus(Guid id, UpdateCompetenciaStatusDto dto, Guid usuarioId)
    {
        var competencia = await _competenciaRepository.GetByIdAsync(id);
        if (competencia == null) throw new Exception("Competência não encontrada");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuário não encontrado");

        // Validar permissão
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, competencia.EmpresaId);
        if (!vinculado && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Sem permissão para alterar esta competência");

        // Regras de negócio para alteração de status (opcional, mas bom ter)
        // Por enquanto permite qualquer transição válida pelo enum

        competencia.Status = dto.Status;
        if (!string.IsNullOrEmpty(dto.Observacoes))
            competencia.Observacoes = dto.Observacoes;

        competencia.DataAtualizacao = DateTime.UtcNow;

        await _competenciaRepository.UpdateAsync(competencia);
        await _auditService.RegistrarEvento("UPDATE_COMPETENCIA_STATUS", "Competencia", competencia.Id.ToString(), usuarioId);
    }

    public async Task<CompetenciaResponseDto> GetById(Guid id, Guid usuarioId)
    {
        var competencia = await _competenciaRepository.GetByIdAsync(id);
        if (competencia == null) throw new Exception("Competência não encontrada");

        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, competencia.EmpresaId);
        if (!vinculado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario?.TipoUsuario != TipoUsuario.Admin)
                throw new Exception("Usuário não tem acesso a esta competência");
        }

        return MapToDto(competencia);
    }

    private CompetenciaResponseDto MapToDto(Competencia c) => new(
        c.Id,
        c.EmpresaId,
        c.Mes,
        c.Ano,
        c.Status,
        c.Status.ToString(),
        c.DataCriacao,
        c.DataAtualizacao,
        c.Observacoes
    );
}
