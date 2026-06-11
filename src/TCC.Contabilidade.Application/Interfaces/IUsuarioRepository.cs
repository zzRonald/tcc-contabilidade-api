using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<User?> ObterPorIdAsync(Guid id);
    Task<User?> ObterPorEmailAsync(string email);
    Task AdicionarAsync(User usuario);
    Task AtualizarAsync(User usuario);
    Task SalvarAlteracoesAsync();
    Task<int> CountClientesByContadorId(Guid contadorId);
}