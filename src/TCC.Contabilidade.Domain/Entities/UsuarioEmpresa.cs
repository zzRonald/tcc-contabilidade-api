namespace TCC.Contabilidade.Domain.Entities;

public class UsuarioEmpresa
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }
    public User? Usuario { get; set; }

    public Guid EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public DateTime DataVinculo { get; set; } = DateTime.UtcNow;
}
