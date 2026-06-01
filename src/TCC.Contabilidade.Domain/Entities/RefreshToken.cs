using System;

namespace TCC.Contabilidade.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public User? Usuario { get; set; }
    public DateTime ExpiraEm { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? RevogadoEm { get; set; }

    public bool IsExpirado => DateTime.UtcNow >= ExpiraEm;
    public bool IsAtivo => RevogadoEm == null && !IsExpirado;

    public RefreshToken()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
    }
}
