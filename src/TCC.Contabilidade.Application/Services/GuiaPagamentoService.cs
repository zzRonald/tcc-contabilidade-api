using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class GuiaPagamentoService
{
    private readonly IGuiaPagamentoRepository _repository;
    private readonly ICompetenciaRepository _competenciaRepository;
    private readonly ITenantContext _tenantContext;
    private readonly AuditService _auditService;
    private readonly DocumentoService _documentoService;
    private readonly IUsuarioRepository _usuarioRepository;

    public GuiaPagamentoService(
        IGuiaPagamentoRepository repository,
        ICompetenciaRepository competenciaRepository,
        ITenantContext tenantContext,
        AuditService auditService,
        DocumentoService documentoService,
        IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _competenciaRepository = competenciaRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _documentoService = documentoService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<GuiaPagamentoResponseDTO> CriarAsync(GuiaPagamentoRequestDTO request)
    {
        var competencia = await _competenciaRepository.GetByIdAsync(request.CompetenciaId);
        if (competencia == null)
            throw new Exception("Competência não encontrada.");

        if (request.Valor < 0)
            throw new Exception("O valor da guia não pode ser negativo.");

        var guia = new GuiaPagamento
        {
            CompetenciaId = request.CompetenciaId,
            Tipo = request.Tipo,
            Valor = request.Valor,
            DataVencimento = request.DataVencimento,
            Observacoes = request.Observacoes,
            DocumentoId = request.DocumentoId
        };

        await _repository.AdicionarAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("CRIAR_GUIA_PAGAMENTO", "GuiaPagamento", guia.Id.ToString());

        return MapToResponse(guia);
    }

    public async Task<GuiaPagamentoResponseDTO> AtualizarAsync(Guid id, UpdateGuiaPagamentoRequestDTO request)
    {
        var guia = await _repository.ObterPorIdAsync(id);
        if (guia == null)
            throw new Exception("Guia de pagamento não encontrada.");

        if (request.Valor < 0)
            throw new Exception("O valor da guia não pode ser negativo.");

        guia.Tipo = request.Tipo;
        guia.Valor = request.Valor;
        guia.DataVencimento = request.DataVencimento;
        guia.Observacoes = request.Observacoes;
        guia.DocumentoId = request.DocumentoId;
        guia.ComprovanteId = request.ComprovanteId;
        guia.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("ATUALIZAR_GUIA_PAGAMENTO", "GuiaPagamento", guia.Id.ToString());

        return MapToResponse(guia);
    }

    public async Task<GuiaPagamentoResponseDTO> EnviarComprovanteAsync(Guid id, Microsoft.AspNetCore.Http.IFormFile arquivo, Guid usuarioId)
    {
        var guia = await _repository.ObterPorIdAsync(id);
        if (guia == null)
            throw new Exception("Guia de pagamento não encontrada.");

        if (guia.Status == StatusGuia.Cancelado)
            throw new Exception("Não é possível enviar comprovante para uma guia cancelada.");

        var uploadDto = new TCC.Contabilidade.Application.DTO.Documentos.UploadDocumentoDto(
            guia.EmpresaId,
            guia.CompetenciaId,
            null,
            arquivo
        );

        var documento = await _documentoService.UploadAsync(uploadDto, usuarioId);

        guia.ComprovanteId = documento.Id;
        guia.Status = StatusGuia.ComprovanteEnviado;
        guia.DataEnvioComprovante = DateTime.UtcNow;
        guia.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("ENVIAR_COMPROVANTE_GUIA", "GuiaPagamento", guia.Id.ToString(), usuarioId);

        return MapToResponse(guia);
    }

    public async Task<GuiaPagamentoResponseDTO> ConfirmarPagamentoAsync(Guid id, Guid usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null || (usuario.TipoUsuario != TipoUsuario.Contador && usuario.TipoUsuario != TipoUsuario.Admin))
            throw new Exception("Apenas contadores ou administradores podem confirmar o pagamento de uma guia.");

        var guia = await _repository.ObterPorIdAsync(id);
        if (guia == null)
            throw new Exception("Guia de pagamento não encontrada.");

        if (guia.Status == StatusGuia.Pago)
            throw new Exception("Esta guia já consta como paga.");

        if (guia.Status == StatusGuia.Cancelado)
            throw new Exception("Não é possível confirmar pagamento de uma guia cancelada.");

        guia.Status = StatusGuia.Pago;
        guia.DataPagamento = DateTime.UtcNow;
        guia.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("CONFIRMAR_PAGAMENTO_GUIA", "GuiaPagamento", guia.Id.ToString(), usuarioId);

        return MapToResponse(guia);
    }

    public async Task<GuiaPagamentoResponseDTO> AtualizarStatusAsync(Guid id, UpdateGuiaPagamentoStatusRequestDTO request)
    {
        var guia = await _repository.ObterPorIdAsync(id);
        if (guia == null)
            throw new Exception("Guia de pagamento não encontrada.");

        guia.Status = request.Status;
        guia.DataPagamento = request.Status == StatusGuia.Pago ? (request.DataPagamento ?? DateTime.UtcNow) : null;
        guia.Observacoes = request.Observacoes ?? guia.Observacoes;
        guia.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("ATUALIZAR_STATUS_GUIA_PAGAMENTO", "GuiaPagamento", guia.Id.ToString());

        return MapToResponse(guia);
    }

    public async Task RemoverAsync(Guid id)
    {
        var guia = await _repository.ObterPorIdAsync(id);
        if (guia == null)
            throw new Exception("Guia de pagamento não encontrada.");

        await _repository.RemoverAsync(guia);
        await _repository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("REMOVER_GUIA_PAGAMENTO", "GuiaPagamento", id.ToString());
    }

    public async Task<GuiaPagamentoResponseDTO?> ObterPorIdAsync(Guid id)
    {
        var guia = await _repository.ObterPorIdAsync(id);
        return guia != null ? MapToResponse(guia) : null;
    }

    public async Task<(IEnumerable<GuiaPagamentoResponseDTO> Itens, PaginationMetadataDTO Paginacao)> ObterPaginadoAsync(GuiaPagamentoFilterDTO filtros)
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

    private static GuiaPagamentoResponseDTO MapToResponse(GuiaPagamento guia)
    {
        return new GuiaPagamentoResponseDTO(
            guia.Id,
            guia.EmpresaId,
            guia.CompetenciaId,
            guia.Competencia?.Mes ?? 0,
            guia.Competencia?.Ano ?? 0,
            guia.Tipo,
            guia.Valor,
            guia.DataVencimento,
            guia.Status,
            guia.DataPagamento,
            guia.DataEnvioComprovante,
            guia.Observacoes,
            guia.DocumentoId,
            guia.ComprovanteId,
            guia.DataCriacao,
            guia.DataVencimento < DateTime.UtcNow && guia.Status == StatusGuia.Pendente
        );
    }
}
