namespace TCC.Contabilidade.Application.Interfaces;

public interface ITenantContext
{
    Guid? TenantId { get; }
}
