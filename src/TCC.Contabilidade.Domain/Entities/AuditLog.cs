namespace TCC.Contabilidade.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public Guid? UsuarioId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string? EntidadeId { get; set; }
    public DateTime DataHora { get; set; }
    public string Ip { get; set; } = string.Empty;
}
