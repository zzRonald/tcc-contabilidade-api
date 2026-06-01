using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class IntegrationService
{
    private readonly IEnumerable<IExternalIntegration> _integrations;

    public IntegrationService(IEnumerable<IExternalIntegration> integrations)
    {
        _integrations = integrations;
    }

    public List<ExternalIntegrationDTO> ListarIntegracoes()
    {
        return _integrations.Select(i => new ExternalIntegrationDTO
        {
            Nome = i.NomeIntegracao,
            Status = i.Ativo ? "Ativo" : "Inativo",
            UltimaVerificacao = DateTime.UtcNow
        }).ToList();
    }
}
