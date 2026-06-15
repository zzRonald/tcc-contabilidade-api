using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class SolicitacaoDocumento : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;
    public TipoDocumento TipoDocumento { get; set; }
    public StatusSolicitacaoDocumento Status { get; set; }
    public string? ObservacaoContador { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public SolicitacaoDocumento()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusSolicitacaoDocumento.Pendente;
    }
}
