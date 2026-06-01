using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class CompanyConfig : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string MoedaPadrao { get; set; } = "BRL";
    public string FormatoData { get; set; } = "dd/MM/yyyy";
    public string Timezone { get; set; } = "America/Sao_Paulo";
    public bool NotificacoesEmail { get; set; } = true;

    // Navigation property
    public Empresa? Empresa { get; set; }
}
