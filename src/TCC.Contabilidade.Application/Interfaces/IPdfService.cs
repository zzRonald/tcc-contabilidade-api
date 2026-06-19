using TCC.Contabilidade.Application.DTO.Relatorios;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateMonthlyReportPdf(RelatorioMensalDTO relatorio);
}
