using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Application.Interfaces; // Adicionado para usar a Interface
using BCrypt.Net;

namespace TCC.Contabilidade.Application.Services;

public class UserService
{
    // Substituímos o AppDbContext pela Interface para quebrar a dependência circular
    private readonly IUsuarioRepository _usuarioRepository;

    public UserService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
    }

    // Registrar usuário
    public async Task<Usuario> RegisterAsync(string nome, string email, string senha, string perfil)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório", nameof(nome));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email é obrigatório", nameof(email));
        if (string.IsNullOrWhiteSpace(senha)) throw new ArgumentException("Senha é obrigatória", nameof(senha));
        if (string.IsNullOrWhiteSpace(perfil)) throw new ArgumentException("Perfil é obrigatório", nameof(perfil));

        // Verifica se email já existe usando o Repositório (Interface)
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuarioExistente != null)
            throw new InvalidOperationException("Email já cadastrado");

        // Hash da senha
        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        var usuario = new Usuario(nome, email, senhaHash, perfil);

        // Adiciona e salva através da Interface
        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return usuario;
    }

    // Validar login e senha
    public async Task<Usuario?> AuthenticateAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return null;

        // Busca o usuário pelo repositório
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuario == null) return null;

        // Verifica o hash da senha
        bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);

        return senhaValida ? usuario : null;
    }
}