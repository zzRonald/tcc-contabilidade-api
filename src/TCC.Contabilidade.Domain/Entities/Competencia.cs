using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class Competencia : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public int Mes { get; set; }
    public int Ano { get; set; }
    public StatusCompetencia Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public string? Observacoes { get; set; }

    public Competencia()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusCompetencia.Aberta;
    }
}
