using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class GuiaPagamento : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;
    public TipoGuia Tipo { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public StatusGuia Status { get; set; }
    public DateTime? DataPagamento { get; set; }
    public DateTime? DataEnvioComprovante { get; set; }
    public string? Observacoes { get; set; }
    public Guid? DocumentoId { get; set; }
    public Documento? Documento { get; set; }
    public Guid? ComprovanteId { get; set; }
    public Documento? Comprovante { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public GuiaPagamento()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusGuia.Pendente;
    }
}
