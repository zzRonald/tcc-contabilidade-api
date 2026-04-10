namespace TCC.Contabilidade.Application.DTO;

public class CnpjResponseDTO
{
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string SituacaoCadastral { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
}