namespace TCC.Contabilidade.Application.Interfaces;

public interface IExternalIntegration
{
    string NomeIntegracao { get; }
    bool Ativo { get; }
    Task<bool> TestarConexaoAsync();
}
