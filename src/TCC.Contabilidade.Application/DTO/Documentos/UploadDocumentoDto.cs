using Microsoft.AspNetCore.Http;

namespace TCC.Contabilidade.Application.DTO.Documentos;

public record UploadDocumentoDto(
    Guid EmpresaId,
    Guid CompetenciaId,
    Guid? SolicitacaoId,
    IFormFile Arquivo
);
