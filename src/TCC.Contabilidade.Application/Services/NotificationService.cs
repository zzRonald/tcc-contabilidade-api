using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IEmailService _emailService;

    public NotificationService(INotificationRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task EnviarNotificacaoAsync(string emailDestino, string mensagem, TipoNotificacao tipo, Guid? usuarioId = null)
    {
        // 1. Enviar E-mail (se falhar, logamos mas não impedimos a gravação se for requisito,
        // mas aqui vamos tentar garantir o envio antes do registro ou tratar conforme o épico)

        try
        {
            string assunto = GetAssuntoPorTipo(tipo);
            await _emailService.SendEmailAsync(emailDestino, assunto, mensagem);
        }
        catch (Exception ex)
        {
            // Log do erro (Poderia usar um ILogger aqui se disponível)
            Console.WriteLine($"Erro ao enviar e-mail de notificação: {ex.Message}");
            // Dependendo do requisito, poderíamos relançar ou apenas seguir.
            // O critério de aceite diz: "Falha no envio não quebra indevidamente a regra principal sem tratamento"
        }

        // 2. Registrar no Banco de Dados
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            EmailDestino = emailDestino,
            Tipo = tipo,
            Mensagem = mensagem,
            DataEnvio = DateTime.UtcNow
            // EmpresaId será preenchido pelo AppDbContext (ApplyTenantId) se houver contexto
        };

        await _repository.AddAsync(notification);
        await _repository.SaveChangesAsync();
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
