using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.DTO;

public record GuiaPagamentoRequestDTO(
    Guid CompetenciaId,
    TipoGuia Tipo,
    decimal Valor,
    DateTime DataVencimento,
    string? Observacoes,
    Guid? DocumentoId
);

public record GuiaPagamentoResponseDTO(
    Guid Id,
    Guid EmpresaId,
    Guid CompetenciaId,
    int Mes,
    int Ano,
    TipoGuia Tipo,
    decimal Valor,
    DateTime DataVencimento,
    StatusGuia Status,
    DateTime? DataPagamento,
    string? Observacoes,
    Guid? DocumentoId,
    DateTime DataCriacao,
    bool EstaVencida
);

public class GuiaPagamentoFilterDTO
{
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public Guid? CompetenciaId { get; set; }
}

public record UpdateGuiaPagamentoStatusRequestDTO(
    StatusGuia Status,
    DateTime? DataPagamento,
    string? Observacoes
);

public record UpdateGuiaPagamentoRequestDTO(
    TipoGuia Tipo,
    decimal Valor,
    DateTime DataVencimento,
    string? Observacoes,
    Guid? DocumentoId
);
