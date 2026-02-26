namespace TCC.Contabilidade.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public string Perfil { get; private set; } // Contador ou Cliente
    public DateTime DataCriacao { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string perfil)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
        DataCriacao = DateTime.UtcNow;
    }
}