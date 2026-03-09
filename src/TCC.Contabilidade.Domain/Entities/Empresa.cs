namespace TCC.Contabilidade.Domain.Entities;

public class Empresa
{
    public Guid Id { get; set; }

    public string Nome { get; set; }

    public string CNPJ { get; set; }

    public Guid ClienteId { get; set; }
}