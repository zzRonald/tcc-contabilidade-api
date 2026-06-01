namespace TCC.Contabilidade.Application.DTO;

public class ExternalIntegrationDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime UltimaVerificacao { get; set; }
}
