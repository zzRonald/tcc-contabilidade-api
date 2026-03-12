namespace TCC.Contabilidade.Application.DTOs;

public class RegisterWithInviteRequest
{
    public string InvitationToken { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}