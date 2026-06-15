using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class SolicitacaoDocumentoService
{
    private readonly ISolicitacaoDocumentoRepository _solicitacaoRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly ICompetenciaRepository _competenciaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AuditService _auditService;

    public SolicitacaoDocumentoService(
        ISolicitacaoDocumentoRepository solicitacaoRepository,
        IEmpresaRepository empresaRepository,
        ICompetenciaRepository competenciaRepository,
        IUsuarioRepository usuarioRepository,
        AuditService auditService)
    {
        _solicitacaoRepository = solicitacaoRepository;
        _empresaRepository = empresaRepository;
        _competenciaRepository = competenciaRepository;
        _usuarioRepository = usuarioRepository;
        _auditService = auditService;
    }

    public async Task Create(CreateSolicitacaoDocumentoDto dto, Guid usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuário não encontrado");

        if (usuario.TipoUsuario != TipoUsuario.Contador && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Sem permissão para solicitar documentos");

        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, dto.EmpresaId);
        if (!vinculado && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Usuário não vinculado a esta empresa");

        var competencia = await _competenciaRepository.GetByIdAsync(dto.CompetenciaId);
        if (competencia == null) throw new Exception("Competência não encontrada");

        if (competencia.EmpresaId != dto.EmpresaId)
            throw new Exception("Competência não pertence à empresa informada");

        var solicitacao = new SolicitacaoDocumento
        {
            EmpresaId = dto.EmpresaId,
            CompetenciaId = dto.CompetenciaId,
            TipoDocumento = dto.TipoDocumento,
            ObservacaoContador = dto.ObservacaoContador
        };

        await _solicitacaoRepository.AddAsync(solicitacao);
        await _auditService.RegistrarEvento("CREATE_SOLICITACAO_DOCUMENTO", "SolicitacaoDocumento", solicitacao.Id.ToString(), usuarioId);
    }

    public async Task<(IEnumerable<SolicitacaoDocumentoResponseDto> Items, PaginationMetadataDTO Metadata)> GetByEmpresa(Guid empresaId, Guid usuarioId, int page, int pageSize)
    {
        await ValidarAcessoEmpresa(empresaId, usuarioId);

        var (items, totalCount) = await _solicitacaoRepository.GetPagedByEmpresaIdAsync(empresaId, page, pageSize);

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

    public async Task<SolicitacaoDocumentoResponseDto> GetById(Guid id, Guid usuarioId)
    {
        var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
        if (solicitacao == null) throw new Exception("Solicitação não encontrada");

        await ValidarAcessoEmpresa(solicitacao.EmpresaId, usuarioId);

        return MapToDto(solicitacao);
    }

    public async Task UpdateStatus(Guid id, UpdateSolicitacaoStatusDto dto, Guid usuarioId)
    {
        var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
        if (solicitacao == null) throw new Exception("Solicitação não encontrada");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuário não encontrado");

        await ValidarAcessoEmpresa(solicitacao.EmpresaId, usuarioId);

        // Regra de negócio simplificada: qualquer pessoa com acesso à empresa pode atualizar status
        // Em um cenário real, talvez apenas o cliente possa marcar como "Enviado" e o contador como "Aprovado"

        solicitacao.Status = dto.Status;
        solicitacao.DataAtualizacao = DateTime.UtcNow;

        await _solicitacaoRepository.UpdateAsync(solicitacao);
        await _auditService.RegistrarEvento("UPDATE_SOLICITACAO_STATUS", "SolicitacaoDocumento", solicitacao.Id.ToString(), usuarioId);
    }

    private async Task ValidarAcessoEmpresa(Guid empresaId, Guid usuarioId)
    {
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId);
        if (!vinculado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario?.TipoUsuario != TipoUsuario.Admin)
                throw new Exception("Usuário não tem acesso a esta empresa");
        }
    }

    private SolicitacaoDocumentoResponseDto MapToDto(SolicitacaoDocumento s) => new(
        s.Id,
        s.EmpresaId,
        s.CompetenciaId,
        s.TipoDocumento,
        s.TipoDocumento.ToString(),
        s.Status,
        s.Status.ToString(),
        s.ObservacaoContador,
        s.DataCriacao,
        s.DataAtualizacao
    );
}
