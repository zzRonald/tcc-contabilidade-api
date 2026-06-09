using BCrypt.Net;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class UserService
{
    // Repositórios
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConviteRepository _conviteRepository;

    // Construtor com injeção de dependência
    public UserService(
        IUsuarioRepository usuarioRepository,
        IConviteRepository conviteRepository)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _conviteRepository = conviteRepository ?? throw new ArgumentNullException(nameof(conviteRepository));
    }

    // =============================
    // Registro com convite
    // =============================
    public async Task<User> RegisterWithInviteAsync(
        string token,
        string nome,
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token de convite é obrigatório");

        var convite = await _conviteRepository.ObterPorTokenAsync(token);

        if (convite == null)
            throw new Exception("Convite inválido");

        if (convite.Utilizado)
            throw new Exception("Convite já utilizado");

        if (convite.Expiracao < DateTime.UtcNow)
            throw new Exception("Convite expirado");

        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuarioExistente != null)
            throw new Exception("Email já cadastrado");

        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        var user = new User(
            nome,
            email,
            senhaHash,
            TipoUsuario.Cliente
        );

        // vincula cliente ao contador
        user.ContadorId = convite.ContadorId;

        await _usuarioRepository.AdicionarAsync(user);

        // marca convite como usado
        convite.Utilizado = true;

        await _usuarioRepository.SalvarAlteracoesAsync();

        return user;
    }

    // =============================
    // Registro normal
    // =============================
    public async Task<User> RegisterAsync(string nome, string email, string senha, string perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));

        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Senha é obrigatória", nameof(senha));

        if (string.IsNullOrWhiteSpace(perfil))
            throw new ArgumentException("Perfil é obrigatório", nameof(perfil));

        // verifica email existente
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuarioExistente != null)
            throw new InvalidOperationException("Email já cadastrado");

        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        var tipoUsuario = Enum.Parse<TipoUsuario>(perfil, true);

        var usuario = new User(nome, email, senhaHash, tipoUsuario);

        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return usuario;
    }

    // =============================
    // Login
    // =============================
    public async Task<User?> AuthenticateAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return null;

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario == null)
            return null;

        bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);

        return senhaValida ? usuario : null;
    }

    public async Task<User?> ObterPorIdAsync(Guid id)
    {
        return await _usuarioRepository.ObterPorIdAsync(id);
    }

    public async Task UpdateProfileAsync(Guid id, string nome, string email)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Usuário não encontrado");

        if (usuario.Email != email)
        {
            var existente = await _usuarioRepository.ObterPorEmailAsync(email);
            if (existente != null && existente.Id != id)
                throw new Exception("Email já está sendo utilizado por outro usuário");
        }

        usuario.Nome = nome;
        usuario.Email = email;

        await _usuarioRepository.AtualizarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();
    }

    public async Task ChangePasswordAsync(Guid id, string senhaAtual, string novaSenha)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Usuário não encontrado");

        if (!BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.SenhaHash))
            throw new Exception("Senha atual incorreta");

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);

        await _usuarioRepository.AtualizarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();
    }
}