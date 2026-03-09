using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public TipoUsuario TipoUsuario { get; set; }

    public bool EmailConfirmado { get; set; }

    public string CodigoConfirmacao { get; set; } = string.Empty;

    public Guid? ContadorId { get; set; }

    public User? Contador { get; set; }

    public ICollection<User>? Clientes { get; set; }

    public User() { }

    public User(string nome, string email, string senhaHash, TipoUsuario tipoUsuario)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        TipoUsuario = tipoUsuario;
        EmailConfirmado = false;
    }
}