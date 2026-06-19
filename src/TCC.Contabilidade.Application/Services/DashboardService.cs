using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConviteRepository _conviteRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IGuiaPagamentoRepository _guiaPagamentoRepository;

    public DashboardService(
        IEmpresaRepository empresaRepository,
        IUsuarioRepository usuarioRepository,
        IConviteRepository conviteRepository,
        IAuditRepository auditRepository,
        IGuiaPagamentoRepository guiaPagamentoRepository)
    {
        _empresaRepository = empresaRepository;
        _usuarioRepository = usuarioRepository;
        _conviteRepository = conviteRepository;
        _auditRepository = auditRepository;
        _guiaPagamentoRepository = guiaPagamentoRepository;
    }

    public async Task<DashboardSummaryDTO> GetResumoAsync(Guid usuarioId)
    {
        var totalEmpresas = await _empresaRepository.CountByUsuarioId(usuarioId);
        var totalUsuarios = await _usuarioRepository.CountClientesByContadorId(usuarioId);
        var convitesPendentes = await _conviteRepository.CountPendentesByContadorId(usuarioId);
        var guiasVencidas = await _guiaPagamentoRepository.CountVencidasByUsuarioIdAsync(usuarioId);

        var atividadesRecentes = await _auditRepository.GetPagedAsync(new AuditLogFilterDTO
        {
            UsuarioId = usuarioId,
            Pagina = 1,
            TamanhoPagina = 5
        });

        return new DashboardSummaryDTO
        {
            TotalEmpresas = totalEmpresas,
            TotalUsuarios = totalUsuarios,
            ConvitesPendentes = convitesPendentes,
            GuiasVencidas = guiasVencidas,
            AtividadesRecentes = atividadesRecentes.Logs.ToList()
        };
    }
}
