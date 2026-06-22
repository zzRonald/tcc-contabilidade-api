using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

using Microsoft.Extensions.Logging;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IGuiaPagamentoRepository _guiaRepository;
    private readonly IObrigacaoRepository _obrigacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository repository,
        IEmailService emailService,
        IGuiaPagamentoRepository guiaRepository,
        IObrigacaoRepository obrigacaoRepository,
        IUsuarioRepository usuarioRepository,
        ILogger<NotificationService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _guiaRepository = guiaRepository;
        _obrigacaoRepository = obrigacaoRepository;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task EnviarNotificacaoAsync(string emailDestino, string mensagem, TipoNotificacao tipo, Guid? usuarioId = null, Guid? referenciaId = null, Guid? empresaId = null)
    {
        // 1. Enviar E-mail
        try
        {
            string assunto = GetAssuntoPorTipo(tipo);
            await _emailService.SendEmailAsync(emailDestino, assunto, mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail de notificação para {email}", emailDestino);
        }

        // 2. Registrar no Banco de Dados
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            ReferenciaId = referenciaId,
            EmpresaId = empresaId ?? Guid.Empty,
            EmailDestino = emailDestino,
            Tipo = tipo,
            Mensagem = mensagem,
            DataEnvio = DateTime.UtcNow
            // EmpresaId será preenchido pelo AppDbContext (ApplyTenantId) se houver contexto
        };

        await _repository.AddAsync(notification);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> PossuiNotificacaoRecenteAsync(Guid referenciaId, TipoNotificacao tipo)
    {
        // Consideramos recente se foi enviada nos últimos 7 dias para evitar spam de vencimento
        var limite = DateTime.UtcNow.AddDays(-7);
        var notificacoes = await _repository.GetByReferenciaIdAsync(referenciaId, tipo);
        return notificacoes.Any(n => n.DataEnvio > limite);
    }

    public async Task ProcessarNotificacoesVencimentoAsync()
    {
        // 1. Processar Guias de Pagamento (Vencimento em 3 dias)
        var guias = await _guiaRepository.ObterGuiasVencimentoProximoAsync(3);
        foreach (var guia in guias)
        {
            if (await PossuiNotificacaoRecenteAsync(guia.Id, TipoNotificacao.Vencimento))
                continue;

            await NotificarUsuariosEmpresaAsync(guia.EmpresaId,
                $"A guia {guia.Tipo} da empresa {guia.Empresa.Nome} vence em {guia.DataVencimento:dd/MM/yyyy}. Valor: R$ {guia.Valor:N2}",
                guia.Id);
        }

        // 2. Processar Obrigações (Vencimento em 3 dias)
        var obrigacoes = await _obrigacaoRepository.ObterObrigacoesVencimentoProximoAsync(3);
        foreach (var obrigacao in obrigacoes)
        {
            if (await PossuiNotificacaoRecenteAsync(obrigacao.Id, TipoNotificacao.Vencimento))
                continue;

            await NotificarUsuariosEmpresaAsync(obrigacao.EmpresaId,
                $"A obrigação {obrigacao.Tipo} ({obrigacao.Descricao}) da empresa {obrigacao.Empresa.Nome} vence em {obrigacao.DataVencimento:dd/MM/yyyy}.",
                obrigacao.Id);
        }
    }

    private async Task NotificarUsuariosEmpresaAsync(Guid empresaId, string mensagem, Guid referenciaId)
    {
        var usuarios = await _usuarioRepository.ObterUsuariosPorEmpresaAsync(empresaId);

        foreach (var usuario in usuarios)
        {
            await EnviarNotificacaoAsync(usuario.Email, mensagem, TipoNotificacao.Vencimento, usuario.Id, referenciaId, empresaId);
        }
    }

    private string GetAssuntoPorTipo(TipoNotificacao tipo)
    {
        return tipo switch
        {
            TipoNotificacao.Convite => "Convite para acessar o Sistema Contábil",
            TipoNotificacao.RecuperacaoSenha => "Recuperação de Senha",
            TipoNotificacao.Vencimento => "Aviso de Vencimento",
            _ => "Notificação do Sistema"
        };
    }
}
