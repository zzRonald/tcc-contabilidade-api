namespace TCC.Contabilidade.Application.DTO;

public class UserAdminResponseDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
