namespace TCC.Contabilidade.Application.DTO.Documentos;

public record AnalisarDocumentoDto(
    Guid DocumentoId,
    bool Aprovado,
    string? MotivoRejeicao
);
