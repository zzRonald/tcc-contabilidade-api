namespace TCC.Contabilidade.Application.DTO;

public class CompanyConfigDto
{
    public string MoedaPadrao { get; set; } = string.Empty;
    public string FormatoData { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public bool NotificacoesEmail { get; set; }
}
