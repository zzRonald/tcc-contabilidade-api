namespace TCC.Contabilidade.Application.DTO;

/// <summary>
/// DTO que representa o resumo dos indicadores para o dashboard.
/// </summary>
public class DashboardSummaryDTO
{
    /// <summary>
    /// Total de empresas vinculadas ao usuário.
    /// </summary>
    public int TotalEmpresas { get; set; }

    /// <summary>
    /// Total de clientes (usuários) vinculados ao contador.
    /// </summary>
    public int TotalUsuarios { get; set; }

    /// <summary>
    /// Quantidade de convites pendentes e não expirados.
    /// </summary>
    public int ConvitesPendentes { get; set; }

    /// <summary>
    /// Quantidade de guias de pagamento vencidas.
    /// </summary>
    public int GuiasVencidas { get; set; }

    /// <summary>
    /// Lista das atividades recentes registradas na auditoria para o usuário.
    /// </summary>
    public List<AuditLogResponseDTO> AtividadesRecentes { get; set; } = new();
}
