using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO.Documentos;

public record CreateSolicitacaoDocumentoDto(
    Guid EmpresaId,
    Guid CompetenciaId,
    TipoDocumento TipoDocumento,
    string? ObservacaoContador
);
