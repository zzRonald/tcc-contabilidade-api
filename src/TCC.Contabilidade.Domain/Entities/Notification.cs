using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class Notification : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid? ReferenciaId { get; set; }
    public string EmailDestino { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
}
