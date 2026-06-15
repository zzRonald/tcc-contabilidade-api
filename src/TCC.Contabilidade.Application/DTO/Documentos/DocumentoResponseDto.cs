namespace TCC.Contabilidade.Application.DTO.Documentos;

public record DocumentoResponseDto(
    Guid Id,
    Guid EmpresaId,
    Guid CompetenciaId,
    Guid? SolicitacaoDocumentoId,
    Guid UsuarioId,
    string Nome,
    long Tamanho,
    string Extensao,
    string MimeType,
    DateTime DataCriacao
);
