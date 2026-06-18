using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class Obrigacao : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;
    public TipoObrigacao Tipo { get; set; }
    public StatusObrigacao Status { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public Obrigacao()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusObrigacao.Pendente;
    }
}
