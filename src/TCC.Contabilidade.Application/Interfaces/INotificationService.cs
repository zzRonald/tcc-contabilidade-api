using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Interfaces;

public interface INotificationService
{
    Task EnviarNotificacaoAsync(string emailDestino, string mensagem, TipoNotificacao tipo, Guid? usuarioId = null);
}
