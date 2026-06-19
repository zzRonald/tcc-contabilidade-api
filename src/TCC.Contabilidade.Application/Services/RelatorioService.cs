using TCC.Contabilidade.Application.DTO.Relatorios;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICompetenciaRepository _competenciaRepository;
    private readonly ISolicitacaoDocumentoRepository _solicitacaoRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IObrigacaoRepository _obrigacaoRepository;
    private readonly IGuiaPagamentoRepository _guiaPagamentoRepository;

    public RelatorioService(
        IEmpresaRepository empresaRepository,
        IUsuarioRepository usuarioRepository,
        ICompetenciaRepository competenciaRepository,
        ISolicitacaoDocumentoRepository solicitacaoRepository,
        IDocumentoRepository documentoRepository,
        IObrigacaoRepository obrigacaoRepository,
        IGuiaPagamentoRepository guiaPagamentoRepository)
    {
        _empresaRepository = empresaRepository;
        _usuarioRepository = usuarioRepository;
        _competenciaRepository = competenciaRepository;
        _solicitacaoRepository = solicitacaoRepository;
        _documentoRepository = documentoRepository;
        _obrigacaoRepository = obrigacaoRepository;
        _guiaPagamentoRepository = guiaPagamentoRepository;
    }

    public async Task<RelatorioMensalDTO> GetRelatorioMensalAsync(Guid empresaId, int mes, int ano, Guid usuarioId)
    {
        // Validar vínculo com a empresa
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId);
        if (!vinculado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario?.TipoUsuario != TipoUsuario.Admin)
            {
                throw new UnauthorizedAccessException("Usuário não tem acesso a esta empresa.");
            }
        }

        var empresa = await _empresaRepository.GetById(empresaId);
        if (empresa == null) throw new KeyNotFoundException("Empresa não encontrada.");

        var competencia = await _competenciaRepository.GetByMesAnoAsync(empresaId, mes, ano);

        var relatorio = new RelatorioMensalDTO
        {
            EmpresaId = empresaId,
            NomeEmpresa = empresa.Nome,
            Mes = mes,
            Ano = ano,
            StatusCompetencia = competencia?.Status.ToString() ?? "Não Iniciada"
        };

        if (competencia == null) return relatorio;

        var competenciaId = competencia.Id;

        // Documentos
        relatorio.Documentos = new DocumentoStatsDTO
        {
            Solicitados = await _solicitacaoRepository.CountByCompetenciaIdAsync(competenciaId),
            Enviados = await _documentoRepository.CountByCompetenciaIdAsync(competenciaId),
            Aprovados = await _documentoRepository.CountByCompetenciaIdAsync(competenciaId, StatusDocumento.Aprovado),
            Rejeitados = await _documentoRepository.CountByCompetenciaIdAsync(competenciaId, StatusDocumento.Rejeitado)
        };

        // Obrigações
        relatorio.Obrigacoes = new ObrigacaoStatsDTO
        {
            Pendentes = await _obrigacaoRepository.CountByCompetenciaIdAsync(competenciaId, StatusObrigacao.Pendente),
            Concluidas = await _obrigacaoRepository.CountByCompetenciaIdAsync(competenciaId, StatusObrigacao.Concluida),
            Atrasadas = await _obrigacaoRepository.CountByCompetenciaIdAsync(competenciaId, apenasAtrasadas: true)
        };

        // Guias de Pagamento
        relatorio.Guias = new GuiaPagamentoStatsDTO
        {
            Abertas = await _guiaPagamentoRepository.CountByCompetenciaIdAsync(competenciaId, StatusGuia.Pendente),
            Pagas = await _guiaPagamentoRepository.CountByCompetenciaIdAsync(competenciaId, StatusGuia.Pago),
            Vencidas = await _guiaPagamentoRepository.CountByCompetenciaIdAsync(competenciaId, apenasVencidas: true)
        };

        return relatorio;
    }
}
