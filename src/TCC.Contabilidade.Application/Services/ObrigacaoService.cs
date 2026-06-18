using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class ObrigacaoService
{
    private readonly IObrigacaoRepository _repository;
    private readonly ICompetenciaRepository _competenciaRepository;
    private readonly ITenantContext _tenantContext;
    private readonly AuditService _auditService;

    public ObrigacaoService(
        IObrigacaoRepository repository,
        ICompetenciaRepository competenciaRepository,
        ITenantContext tenantContext,
        AuditService auditService)
    {
        _repository = repository;
        _competenciaRepository = competenciaRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<ObrigacaoResponseDTO> CriarAsync(ObrigacaoRequestDTO request)
    {
        var competencia = await _competenciaRepository.GetByIdAsync(request.CompetenciaId);
        if (competencia == null)
            throw new Exception("Competência não encontrada.");

        var obrigacao = new Obrigacao
        {
            CompetenciaId = request.CompetenciaId,
            Tipo = request.Tipo,
            DataVencimento = request.DataVencimento,
            Descricao = request.Descricao,
            Observacoes = request.Observacoes
        };

        await _repository.AdicionarAsync(obrigacao);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("Criar", "Obrigacao", obrigacao.Id.ToString());

        return MapToResponse(obrigacao);
    }

    public async Task<ObrigacaoResponseDTO> AtualizarAsync(Guid id, UpdateObrigacaoRequestDTO request)
    {
        var obrigacao = await _repository.ObterPorIdAsync(id);
        if (obrigacao == null)
            throw new Exception("Obrigação não encontrada.");

        obrigacao.Tipo = request.Tipo;
        obrigacao.DataVencimento = request.DataVencimento;
        obrigacao.Descricao = request.Descricao;
        obrigacao.Observacoes = request.Observacoes;
        obrigacao.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(obrigacao);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("Atualizar", "Obrigacao", obrigacao.Id.ToString());

        return MapToResponse(obrigacao);
    }

    public async Task<ObrigacaoResponseDTO> AtualizarStatusAsync(Guid id, UpdateObrigacaoStatusRequestDTO request)
    {
        var obrigacao = await _repository.ObterPorIdAsync(id);
        if (obrigacao == null)
            throw new Exception("Obrigação não encontrada.");

        obrigacao.Status = request.Status;
        obrigacao.Observacoes = request.Observacoes;
        obrigacao.DataAtualizacao = DateTime.UtcNow;

        if (request.Status == StatusObrigacao.Concluida)
        {
            obrigacao.DataConclusao = DateTime.UtcNow;
        }
        else
        {
            obrigacao.DataConclusao = null;
        }

        await _repository.AtualizarAsync(obrigacao);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("AtualizarStatus", "Obrigacao", obrigacao.Id.ToString());

        return MapToResponse(obrigacao);
    }

    public async Task RemoverAsync(Guid id)
    {
        var obrigacao = await _repository.ObterPorIdAsync(id);
        if (obrigacao == null)
            throw new Exception("Obrigação não encontrada.");

        await _repository.RemoverAsync(obrigacao);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("Remover", "Obrigacao", id.ToString());
    }

    public async Task<ObrigacaoResponseDTO?> ObterPorIdAsync(Guid id)
    {
        var obrigacao = await _repository.ObterPorIdAsync(id);
        return obrigacao != null ? MapToResponse(obrigacao) : null;
    }

    public async Task<(IEnumerable<ObrigacaoResponseDTO> Itens, PaginationMetadataDTO Paginacao)> ObterPaginadoAsync(ObrigacaoFilterDTO filtros)
    {
        var empresaId = _tenantContext.TenantId ?? throw new Exception("Tenant não identificado.");

        var (itens, total) = await _repository.ObterPaginadoAsync(
            empresaId,
            filtros.Pagina,
            filtros.TamanhoPagina,
            filtros.CompetenciaId);

        var response = itens.Select(MapToResponse);

        var paginacao = new PaginationMetadataDTO
        {
            PaginaAtual = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalRegistros = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };

        return (response, paginacao);
    }

    private static ObrigacaoResponseDTO MapToResponse(Obrigacao obrigacao)
    {
        return new ObrigacaoResponseDTO(
            obrigacao.Id,
            obrigacao.EmpresaId,
            obrigacao.CompetenciaId,
            obrigacao.Competencia?.Mes ?? 0,
            obrigacao.Competencia?.Ano ?? 0,
            obrigacao.Tipo,
            obrigacao.Status,
            obrigacao.DataVencimento,
            obrigacao.DataConclusao,
            obrigacao.Descricao,
            obrigacao.Observacoes,
            obrigacao.DataCriacao,
            obrigacao.DataVencimento < DateTime.UtcNow && obrigacao.Status != StatusObrigacao.Concluida
        );
    }
}
