using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO.Documentos;

public record UpdateSolicitacaoStatusDto(
    StatusSolicitacaoDocumento Status
);
