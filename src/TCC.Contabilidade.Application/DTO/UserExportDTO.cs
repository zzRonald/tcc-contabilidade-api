namespace TCC.Contabilidade.Application.DTO;

public class UserExportDTO
{
    public UserExportDadosPessoaisDTO DadosPessoais { get; set; } = new();
    public IEnumerable<UserExportEmpresaDTO> EmpresasVinculadas { get; set; } = [];
    public UserExportSegurancaDTO Seguranca { get; set; } = new();
    public DateTime DataExportacao { get; set; }
    public string AvisoLegal { get; set; } = string.Empty;
}

public class UserExportDadosPessoaisDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class UserExportEmpresaDTO
{
    public string Nome { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
}

public class UserExportSegurancaDTO
{
    public bool EmailConfirmado { get; set; }
    public bool PossuiContadorVinculado { get; set; }
}
