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
    Task<List<User>> ObterUsuariosPorEmpresaAsync(Guid empresaId);
    Task<(IEnumerable<User> Usuarios, int Total)> ObterUsuariosPaginadosAsync(
        string? nome = null,
        string? email = null,
        string? tipoUsuario = null,
        bool? ativo = null,
        Guid? contadorId = null,
        int pagina = 1,
        int tamanhoPagina = 10);
}