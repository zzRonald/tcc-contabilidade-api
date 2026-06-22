namespace TCC.Contabilidade.Application.DTO;

public class UserFilterDTO
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? TipoUsuario { get; set; }
    public bool? Ativo { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}
