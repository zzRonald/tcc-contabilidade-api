using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.API.Services;

public class VencimentoBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VencimentoBackgroundService> _logger;

    public VencimentoBackgroundService(IServiceProvider serviceProvider, ILogger<VencimentoBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Vencimento Background Service está iniciando.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Executando verificação de vencimentos: {time}", DateTimeOffset.Now);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    await notificationService.ProcessarNotificacoesVencimentoAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar notificações de vencimento.");
            }

            // Executa uma vez por dia
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        _logger.LogInformation("Vencimento Background Service está parando.");
    }
}
