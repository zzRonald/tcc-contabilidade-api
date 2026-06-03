namespace TCC.Contabilidade.Application.DTO;

public class AuditLogResponseDTO
{
    public int Id { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string? EntidadeId { get; set; }
    public DateTime DataHora { get; set; }
    public string Ip { get; set; } = string.Empty;
}

public class AuditLogFilterDTO
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? Acao { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}
