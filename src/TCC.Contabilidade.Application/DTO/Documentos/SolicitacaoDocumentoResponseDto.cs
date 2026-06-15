using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO.Documentos;

public record SolicitacaoDocumentoResponseDto(
    Guid Id,
    Guid EmpresaId,
    Guid CompetenciaId,
    TipoDocumento TipoDocumento,
    string TipoDocumentoDescricao,
    StatusSolicitacaoDocumento Status,
    string StatusDescricao,
    string? ObservacaoContador,
    DateTime DataCriacao,
    DateTime? DataAtualizacao
);
