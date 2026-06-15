using Microsoft.Extensions.Configuration;
using TCC.Contabilidade.Application.DTO.Documentos;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class DocumentoService
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ISolicitacaoDocumentoRepository _solicitacaoRepository;
    private readonly IStorageService _storageService;
    private readonly AuditService _auditService;
    private readonly IConfiguration _configuration;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICompetenciaRepository _competenciaRepository;

    public DocumentoService(
        IDocumentoRepository documentoRepository,
        ISolicitacaoDocumentoRepository solicitacaoRepository,
        IStorageService storageService,
        AuditService auditService,
        IConfiguration configuration,
        IEmpresaRepository empresaRepository,
        IUsuarioRepository usuarioRepository,
        ICompetenciaRepository competenciaRepository)
    {
        _documentoRepository = documentoRepository;
        _solicitacaoRepository = solicitacaoRepository;
        _storageService = storageService;
        _auditService = auditService;
        _configuration = configuration;
        _empresaRepository = empresaRepository;
        _usuarioRepository = usuarioRepository;
        _competenciaRepository = competenciaRepository;
    }

    public async Task<DocumentoResponseDto> AnalisarAsync(AnalisarDocumentoDto dto, Guid usuarioId)
    {
        // 1. Validar se o usuário é contador ou admin
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null || (usuario.TipoUsuario != TipoUsuario.Contador && usuario.TipoUsuario != TipoUsuario.Admin))
            throw new Exception("Apenas contadores ou administradores podem analisar documentos");

        // 2. Obter documento
        var documento = await _documentoRepository.GetByIdAsync(dto.DocumentoId);
        if (documento == null)
            throw new Exception("Documento não encontrado");

        // 3. Validar se o documento já foi aprovado (apenas admin pode re-analisar)
        if (documento.Status == StatusDocumento.Aprovado && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Este documento já foi aprovado e não pode ser alterado");

        // 4. Validar acesso à empresa do documento
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, documento.EmpresaId);
        if (!vinculado && usuario.TipoUsuario != TipoUsuario.Admin)
            throw new Exception("Usuário não tem acesso a esta empresa");

        // 5. Se rejeitado, exigir motivo
        if (!dto.Aprovado && string.IsNullOrWhiteSpace(dto.MotivoRejeicao))
            throw new Exception("O motivo da rejeição é obrigatório");

        // 6. Atualizar status
        documento.Status = dto.Aprovado ? StatusDocumento.Aprovado : StatusDocumento.Rejeitado;
        documento.AnalisadoPorId = usuarioId;
        documento.DataAnalise = DateTime.UtcNow;
        documento.MotivoRejeicao = dto.Aprovado ? null : dto.MotivoRejeicao;

        // 7. Se houver solicitação e for aprovado, podemos marcar como concluída?
        // O upload já marca como Enviado. Se for Rejeitado, talvez devesse voltar para Enviado (já está) ou Pendente?
        // Se rejeitado, a solicitação deve permitir novo envio.
        if (documento.SolicitacaoDocumentoId.HasValue)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(documento.SolicitacaoDocumentoId.Value);
            if (solicitacao != null)
            {
                if (dto.Aprovado)
                {
                    solicitacao.Status = StatusSolicitacaoDocumento.Concluido;
                }
                else
                {
                    // Se rejeitado, volta para pendente para que o cliente envie novamente
                    solicitacao.Status = StatusSolicitacaoDocumento.Pendente;
                    solicitacao.ObservacaoContador = $"Documento rejeitado: {dto.MotivoRejeicao}";
                }
                solicitacao.DataAtualizacao = DateTime.UtcNow;
                await _solicitacaoRepository.UpdateAsync(solicitacao);
            }
        }

        await _documentoRepository.SaveChangesAsync();
        await _auditService.RegistrarEvento(
            dto.Aprovado ? "APROVAR_DOCUMENTO" : "REJEITAR_DOCUMENTO",
            "Documento",
            documento.Id.ToString(),
            usuarioId);

        return MapToDto(documento);
    }

    public async Task<DocumentoResponseDto> UploadAsync(UploadDocumentoDto dto, Guid usuarioId)
    {
        // 1. Validar acesso à empresa
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, dto.EmpresaId);
        if (!vinculado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario?.TipoUsuario != TipoUsuario.Admin)
                throw new Exception("Usuário não tem acesso a esta empresa");
        }

        // 2. Validar se a competência pertence à empresa
        var competencia = await _competenciaRepository.GetByIdAsync(dto.CompetenciaId);
        if (competencia == null || competencia.EmpresaId != dto.EmpresaId)
            throw new Exception("Competência inválida para esta empresa");

        // 3. Validar arquivo (tamanho e extensão)
        ValidarArquivo(dto.Arquivo);

        // 4. Salvar arquivo físico (retorna apenas o nome do arquivo único)
        string nomeArquivoSalvo;
        using (var stream = dto.Arquivo.OpenReadStream())
        {
            nomeArquivoSalvo = await _storageService.SaveAsync(stream, dto.Arquivo.FileName);
        }

        try
        {
            // 5. Persistir metadados
            var documento = new Documento
            {
                EmpresaId = dto.EmpresaId,
                CompetenciaId = dto.CompetenciaId,
                SolicitacaoDocumentoId = dto.SolicitacaoId,
                UsuarioId = usuarioId,
                Nome = dto.Arquivo.FileName,
                CaminhoArquivo = nomeArquivoSalvo, // Armazenando apenas o nome relativo/único
                Tamanho = dto.Arquivo.Length,
                Extensao = Path.GetExtension(dto.Arquivo.FileName).ToLower(),
                MimeType = dto.Arquivo.ContentType
            };

            await _documentoRepository.AddAsync(documento);

            // 6. Atualizar status da solicitação se houver
            if (dto.SolicitacaoId.HasValue)
            {
                var solicitacao = await _solicitacaoRepository.GetByIdAsync(dto.SolicitacaoId.Value);
                if (solicitacao != null)
                {
                    if (solicitacao.EmpresaId != dto.EmpresaId)
                        throw new Exception("Solicitação não pertence à empresa informada");

                    if (solicitacao.Status == StatusSolicitacaoDocumento.Concluido)
                        throw new Exception("Esta solicitação já foi concluída e não aceita novos documentos");

                    solicitacao.Status = StatusSolicitacaoDocumento.Enviado;
                    solicitacao.DataAtualizacao = DateTime.UtcNow;
                    await _solicitacaoRepository.UpdateAsync(solicitacao);
                }
            }

            await _documentoRepository.SaveChangesAsync();
            await _auditService.RegistrarEvento("UPLOAD_DOCUMENTO", "Documento", documento.Id.ToString(), usuarioId);

            return MapToDto(documento);
        }
        catch (Exception)
        {
            // Rollback arquivo físico se falhar o banco
            await _storageService.DeleteAsync(nomeArquivoSalvo);
            throw;
        }
    }

    private void ValidarArquivo(Microsoft.AspNetCore.Http.IFormFile arquivo)
    {
        var maxSizeBytes = _configuration.GetValue<long>("MaxFileSizeInBytes", 5242880); // Default 5MB
        if (arquivo.Length > maxSizeBytes)
            throw new Exception($"Arquivo excede o limite de {maxSizeBytes / 1024 / 1024}MB");

        var allowedExtensions = _configuration.GetValue<string>("AllowedExtensions", ".pdf,.jpg,.jpeg,.png")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().ToLower());

        var extension = Path.GetExtension(arquivo.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            throw new Exception("Extensão de arquivo não permitida");
    }

    private DocumentoResponseDto MapToDto(Documento d) => new(
        d.Id,
        d.EmpresaId,
        d.CompetenciaId,
        d.SolicitacaoDocumentoId,
        d.UsuarioId,
        d.Nome,
        d.Tamanho,
        d.Extensao,
        d.MimeType,
        d.DataCriacao,
        d.Status,
        d.AnalisadoPorId,
        d.DataAnalise,
        d.MotivoRejeicao
    );
}
