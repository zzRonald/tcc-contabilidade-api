namespace TCC.Contabilidade.Domain.Entities;

public class Empresa
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string CNPJ { get; set; } = string.Empty;

    public Guid ClienteId { get; set; }

    public User? Cliente { get; set; }
}