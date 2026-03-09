namespace TCC.Contabilidade.Domain.Entities;

public class Documento
{
    public Guid Id { get; set; }

    public string Nome { get; set; }

    public string CaminhoArquivo { get; set; }

    public Guid ClienteId { get; set; }
}