namespace TCC.Contabilidade.Domain.Entities;

public class Funcionario
{
    public Guid Id { get; set; }

    public string Nome { get; set; }

    public string CPF { get; set; }

    public decimal Salario { get; set; }

    public DateTime DataAdmissao { get; set; }

    public DateTime? DataFerias { get; set; }

    public Guid EmpresaId { get; set; }
}