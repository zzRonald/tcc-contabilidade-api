using BCrypt.Net;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Application.Utils;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Enums;

namespace TCC.Contabilidade.Application.Services;

public class UserService
{
    // Repositórios
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConviteRepository _conviteRepository;
    private readonly AuditService _auditService;

    // Construtor com injeção de dependência
    public UserService(
        IUsuarioRepository usuarioRepository,
        IConviteRepository conviteRepository,
        AuditService auditService)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _conviteRepository = conviteRepository ?? throw new ArgumentNullException(nameof(conviteRepository));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
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

    // =============================
    // Gestão Administrativa
    // =============================

    public async Task<(IEnumerable<UserAdminResponseDTO> Usuarios, PaginationMetadataDTO Paginacao)> ObterUsuariosPaginadosAsync(
        UserFilterDTO filtros,
        Guid currentUserId,
        TipoUsuario currentUserRole)
    {
        Guid? contadorId = null;

        // Se for contador, filtra apenas pelos seus clientes
        if (currentUserRole == TipoUsuario.Contador)
        {
            contadorId = currentUserId;
            // Contador não deve ver outros contadores ou admins, força filtro para Cliente
            filtros.TipoUsuario = TipoUsuario.Cliente.ToString();
        }
        else if (currentUserRole != TipoUsuario.Admin)
        {
            throw new UnauthorizedAccessException("Usuário não autorizado a listar usuários.");
        }

        var (usuarios, total) = await _usuarioRepository.ObterUsuariosPaginadosAsync(
            filtros.Nome,
            filtros.Email,
            filtros.TipoUsuario,
            filtros.Ativo,
            contadorId,
            filtros.Pagina,
            filtros.TamanhoPagina
        );

        var dtos = usuarios.Select(u => new UserAdminResponseDTO
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = PrivacyUtils.MaskEmail(u.Email),
            TipoUsuario = u.TipoUsuario.ToString(),
            Ativo = u.Ativo
        });

        var paginacao = new PaginationMetadataDTO
        {
            PaginaAtual = filtros.Pagina,
            TamanhoPagina = filtros.TamanhoPagina,
            TotalRegistros = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtros.TamanhoPagina)
        };

        return (dtos, paginacao);
    }

    public async Task UpdateStatusAsync(Guid id, bool ativo, Guid executorId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Usuário não encontrado");

        var executor = await _usuarioRepository.ObterPorIdAsync(executorId)
            ?? throw new Exception("Executor não encontrado");

        // Regras de negócio
        if (executor.TipoUsuario == TipoUsuario.Contador)
        {
            if (usuario.ContadorId != executorId)
                throw new UnauthorizedAccessException("Contador só pode alterar status de seus próprios clientes.");
        }
        else if (executor.TipoUsuario != TipoUsuario.Admin)
        {
            throw new UnauthorizedAccessException("Permissão insuficiente para alterar status de usuário.");
        }

        if (usuario.Id == executorId && !ativo)
            throw new InvalidOperationException("Um usuário não pode inativar a si mesmo.");

        usuario.Ativo = ativo;

        await _usuarioRepository.AtualizarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        string acao = ativo ? "Ativação de Usuário" : "Inativação de Usuário";
        await _auditService.RegistrarEvento(acao, "User", usuario.Id.ToString(), executorId);
    }

    public async Task<object> ExportarDadosPessoaisAsync(Guid usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new Exception("Usuário não encontrado");

        return new
        {
            DadosPessoais = new
            {
                usuario.Nome,
                usuario.Email,
                Perfil = usuario.TipoUsuario.ToString(),
                Status = usuario.Ativo ? "Ativo" : "Inativo"
            },
            Seguranca = new
            {
                EmailConfirmado = usuario.EmailConfirmado,
                PossuiContadorVinculado = usuario.ContadorId.HasValue
            },
            DataExportacao = DateTime.UtcNow,
            AvisoLegal = "Este arquivo contém seus dados pessoais tratados pelo sistema, em conformidade com a LGPD (Direito de Portabilidade)."
        };
    }

    public async Task UpdateRoleAsync(Guid id, string novoPerfil, Guid executorId)
    {
        if (!Enum.TryParse<TipoUsuario>(novoPerfil, true, out var perfilEnum))
            throw new ArgumentException("Perfil inválido.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Usuário não encontrado");

        var executor = await _usuarioRepository.ObterPorIdAsync(executorId)
            ?? throw new Exception("Executor não encontrado");

        // Regras de Segurança contra Escalação de Privilégios
        if (executor.TipoUsuario == TipoUsuario.Contador)
        {
            // Contador só mexe em seus clientes
            if (usuario.ContadorId != executorId)
                throw new UnauthorizedAccessException("Contador só pode gerenciar perfis de seus próprios clientes.");

            // Contador só pode alternar entre Cliente e Contador (opcional, mas seguro)
            // No escopo, diz "Cliente nunca deve conseguir alterar perfil próprio para contador ou DEV"
            // E "Separar regras para DEV, Contador e Cliente"
            if (perfilEnum == TipoUsuario.Admin)
                throw new UnauthorizedAccessException("Contadores não podem promover usuários para Administrador.");
        }
        else if (executor.TipoUsuario != TipoUsuario.Admin)
        {
            throw new UnauthorizedAccessException("Permissão insuficiente para alterar perfil de usuário.");
        }

        var perfilAnterior = usuario.TipoUsuario;
        usuario.TipoUsuario = perfilEnum;

        await _usuarioRepository.AtualizarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento(
            $"Alteração de Perfil de {perfilAnterior} para {perfilEnum}",
            "User",
            usuario.Id.ToString(),
            executorId);
    }
}