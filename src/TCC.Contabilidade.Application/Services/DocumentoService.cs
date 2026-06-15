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
        d.DataCriacao
    );
}
