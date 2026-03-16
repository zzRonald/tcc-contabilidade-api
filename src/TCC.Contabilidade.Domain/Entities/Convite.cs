namespace TCC.Contabilidade.Domain.Entities;

public class Convite
{
    public Guid Id { get; set; }

    public string EmailCliente { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public bool Utilizado { get; set; }

    public Guid ContadorId { get; set; }

    public DateTime Expiracao { get; set; }

    public bool Usado { get; set; }
}