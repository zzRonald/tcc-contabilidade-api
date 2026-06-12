using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO.Competencias;

public record CompetenciaResponseDto(
    Guid Id,
    Guid EmpresaId,
    int Mes,
    int Ano,
    StatusCompetencia Status,
    string StatusDescricao,
    DateTime DataCriacao,
    DateTime? DataAtualizacao,
    string? Observacoes
);

public record CreateCompetenciaDto(
    Guid EmpresaId,
    int Mes,
    int Ano,
    string? Observacoes
);

public record UpdateCompetenciaStatusDto(
    StatusCompetencia Status,
    string? Observacoes
);
