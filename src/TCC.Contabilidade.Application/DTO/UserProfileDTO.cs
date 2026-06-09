namespace TCC.Contabilidade.Application.DTO;

public record UpdateProfileRequest(string Nome, string Email);

public record ChangePasswordRequest(string SenhaAtual, string NovaSenha);
