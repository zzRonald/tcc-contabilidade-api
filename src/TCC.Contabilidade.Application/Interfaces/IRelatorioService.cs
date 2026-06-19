using TCC.Contabilidade.Application.DTO.Relatorios;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioMensalDTO> GetRelatorioMensalAsync(Guid empresaId, int mes, int ano, Guid usuarioId);
    Task<byte[]> GetRelatorioMensalPdfAsync(Guid empresaId, int mes, int ano, Guid usuarioId);
}
