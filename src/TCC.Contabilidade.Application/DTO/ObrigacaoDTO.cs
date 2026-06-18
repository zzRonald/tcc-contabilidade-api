using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO;

public record ObrigacaoRequestDTO(
    Guid CompetenciaId,
    TipoObrigacao Tipo,
    DateTime DataVencimento,
    string Descricao,
    string? Observacoes
);

public record UpdateObrigacaoRequestDTO(
    TipoObrigacao Tipo,
    DateTime DataVencimento,
    string Descricao,
    string? Observacoes
);

public record UpdateObrigacaoStatusRequestDTO(
    StatusObrigacao Status,
    string? Observacoes
);

public record ObrigacaoResponseDTO(
    Guid Id,
    Guid EmpresaId,
    Guid CompetenciaId,
    int MesCompetencia,
    int AnoCompetencia,
    TipoObrigacao Tipo,
    StatusObrigacao Status,
    DateTime DataVencimento,
    DateTime? DataConclusao,
    string Descricao,
    string? Observacoes,
    DateTime DataCriacao,
    bool EstaVencida
);

public record ObrigacaoFilterDTO(
    Guid? CompetenciaId,
    int Pagina = 1,
    int TamanhoPagina = 10
);
