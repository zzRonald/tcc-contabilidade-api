using Moq;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Services;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;
using Xunit;

namespace TCC.Contabilidade.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IGuiaPagamentoRepository> _guiaRepoMock;
    private readonly Mock<IObrigacaoRepository> _obrigacaoRepoMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _guiaRepoMock = new Mock<IGuiaPagamentoRepository>();
        _obrigacaoRepoMock = new Mock<IObrigacaoRepository>();
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<NotificationService>>();

        _service = new NotificationService(
            _notificationRepoMock.Object,
            _emailServiceMock.Object,
            _guiaRepoMock.Object,
            _obrigacaoRepoMock.Object,
            _usuarioRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task EnviarNotificacaoAsync_ShouldSaveNotificationWithReferenciaIdAndEmpresaId()
    {
        // Arrange
        var email = "test@example.com";
        var mensagem = "Teste";
        var tipo = TipoNotificacao.Vencimento;
        var usuarioId = Guid.NewGuid();
        var referenciaId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();

        // Act
        await _service.EnviarNotificacaoAsync(email, mensagem, tipo, usuarioId, referenciaId, empresaId);

        // Assert
        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n =>
            n.EmailDestino == email &&
            n.Mensagem == mensagem &&
            n.Tipo == tipo &&
            n.UsuarioId == usuarioId &&
            n.ReferenciaId == referenciaId &&
            n.EmpresaId == empresaId)), Times.Once);

        _notificationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _emailServiceMock.Verify(e => e.SendEmailAsync(email, It.IsAny<string>(), mensagem), Times.Once);
    }

    [Fact]
    public async Task PossuiNotificacaoRecenteAsync_ShouldReturnTrue_WhenNotificationExistsWithin7Days()
    {
        // Arrange
        var referenciaId = Guid.NewGuid();
        var tipo = TipoNotificacao.Vencimento;
        var notificacoes = new List<Notification>
        {
            new Notification { DataEnvio = DateTime.UtcNow.AddDays(-2) }
        };

        _notificationRepoMock.Setup(r => r.GetByReferenciaIdAsync(referenciaId, tipo))
            .ReturnsAsync(notificacoes);

        // Act
        var result = await _service.PossuiNotificacaoRecenteAsync(referenciaId, tipo);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PossuiNotificacaoRecenteAsync_ShouldReturnFalse_WhenNotificationIsOlderThan7Days()
    {
        // Arrange
        var referenciaId = Guid.NewGuid();
        var tipo = TipoNotificacao.Vencimento;
        var notificacoes = new List<Notification>
        {
            new Notification { DataEnvio = DateTime.UtcNow.AddDays(-8) }
        };

        _notificationRepoMock.Setup(r => r.GetByReferenciaIdAsync(referenciaId, tipo))
            .ReturnsAsync(notificacoes);

        // Act
        var result = await _service.PossuiNotificacaoRecenteAsync(referenciaId, tipo);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessarNotificacoesVencimentoAsync_ShouldNotNotify_WhenAlreadyNotifiedRecently()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var guiaId = Guid.NewGuid();
        var guias = new List<GuiaPagamento>
        {
            new GuiaPagamento { Id = guiaId, EmpresaId = empresaId, Tipo = TipoGuia.DAS, DataVencimento = DateTime.UtcNow.AddDays(2), Empresa = new Empresa { Nome = "Empresa Teste" } }
        };

        _guiaRepoMock.Setup(r => r.ObterGuiasVencimentoProximoAsync(3))
            .ReturnsAsync(guias);

        _obrigacaoRepoMock.Setup(r => r.ObterObrigacoesVencimentoProximoAsync(3))
            .ReturnsAsync(new List<Obrigacao>());

        _notificationRepoMock.Setup(r => r.GetByReferenciaIdAsync(guiaId, TipoNotificacao.Vencimento))
            .ReturnsAsync(new List<Notification> { new Notification { DataEnvio = DateTime.UtcNow.AddDays(-1) } });

        // Act
        await _service.ProcessarNotificacoesVencimentoAsync();

        // Assert
        _usuarioRepoMock.Verify(r => r.ObterUsuariosPorEmpresaAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ProcessarNotificacoesVencimentoAsync_ShouldNotifyUsers_WhenNearlyDueAndNotNotified()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var guiaId = Guid.NewGuid();
        var guias = new List<GuiaPagamento>
        {
            new GuiaPagamento { Id = guiaId, EmpresaId = empresaId, Tipo = TipoGuia.DAS, DataVencimento = DateTime.UtcNow.AddDays(2), Empresa = new Empresa { Nome = "Empresa Teste" } }
        };

        var usuarios = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "user1@test.com" },
            new User { Id = Guid.NewGuid(), Email = "user2@test.com" }
        };

        _guiaRepoMock.Setup(r => r.ObterGuiasVencimentoProximoAsync(3))
            .ReturnsAsync(guias);

        _obrigacaoRepoMock.Setup(r => r.ObterObrigacoesVencimentoProximoAsync(3))
            .ReturnsAsync(new List<Obrigacao>());

        _notificationRepoMock.Setup(r => r.GetByReferenciaIdAsync(guiaId, TipoNotificacao.Vencimento))
            .ReturnsAsync(new List<Notification>());

        _usuarioRepoMock.Setup(r => r.ObterUsuariosPorEmpresaAsync(empresaId))
            .ReturnsAsync(usuarios);

        // Act
        await _service.ProcessarNotificacoesVencimentoAsync();

        // Assert
        _notificationRepoMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Exactly(2));
        _emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }
}
