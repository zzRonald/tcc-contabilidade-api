namespace TCC.Contabilidade.Domain.Entities;

public class Empresa
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string CNPJ { get; set; } = string.Empty;

    public CompanyConfig? Config { get; set; }

    public ICollection<UsuarioEmpresa> UsuariosEmpresas { get; set; } = new List<UsuarioEmpresa>();
}
