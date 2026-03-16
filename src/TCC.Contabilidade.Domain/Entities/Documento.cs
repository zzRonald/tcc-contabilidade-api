namespace TCC.Contabilidade.Domain.Entities;

public class Documento
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string CaminhoArquivo { get; set; } = string.Empty;

    public Guid ClienteId { get; set; }
}