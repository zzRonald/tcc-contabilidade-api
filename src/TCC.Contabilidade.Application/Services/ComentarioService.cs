using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class ComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IGuiaPagamentoRepository _guiaPagamentoRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly AuditService _auditService;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IDocumentoRepository documentoRepository,
        IGuiaPagamentoRepository guiaPagamentoRepository,
        IEmpresaRepository empresaRepository,
        AuditService auditService)
    {
        _comentarioRepository = comentarioRepository;
        _documentoRepository = documentoRepository;
        _guiaPagamentoRepository = guiaPagamentoRepository;
        _empresaRepository = empresaRepository;
        _auditService = auditService;
    }

    public async Task<ComentarioResponseDto> AdicionarComentarioAsync(CreateComentarioDto dto, Guid usuarioId)
    {
        if (string.IsNullOrWhiteSpace(dto.Texto))
            throw new Exception("O texto do comentário não pode estar vazio.");

        Guid empresaId;
        string entidade;
        string entidadeId;

        if (dto.DocumentoId.HasValue)
        {
            var documento = await _documentoRepository.GetByIdAsync(dto.DocumentoId.Value);
            if (documento == null) throw new Exception("Documento não encontrado.");
            empresaId = documento.EmpresaId;
            entidade = "Documento";
            entidadeId = documento.Id.ToString();
        }
        else if (dto.GuiaPagamentoId.HasValue)
        {
            var guia = await _guiaPagamentoRepository.ObterPorIdAsync(dto.GuiaPagamentoId.Value);
            if (guia == null) throw new Exception("Guia de pagamento não encontrada.");
            empresaId = guia.EmpresaId;
            entidade = "GuiaPagamento";
            entidadeId = guia.Id.ToString();
        }
        else
        {
            throw new Exception("É necessário informar um Documento ou uma Guia de Pagamento.");
        }

        // Validar vínculo do usuário com a empresa (Segurança Contextual)
        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId);
        if (!vinculado)
            throw new Exception("Você não tem permissão para comentar neste recurso.");

        var comentario = new Comentario
        {
            UsuarioId = usuarioId,
            EmpresaId = empresaId,
            DocumentoId = dto.DocumentoId,
            GuiaPagamentoId = dto.GuiaPagamentoId,
            Texto = dto.Texto.Trim(),
            DataCriacao = DateTime.UtcNow
        };

        await _comentarioRepository.AddAsync(comentario);
        await _comentarioRepository.SaveChangesAsync();

        await _auditService.RegistrarEvento("CRIAR_COMENTARIO", entidade, entidadeId, usuarioId);

        // O repositório retorna o usuário no Include, mas como acabamos de criar,
        // precisamos garantir que os dados do usuário estejam disponíveis ou buscar novamente.
        // Como o ComentarioRepository.AddAsync não faz o reload, vamos retornar o DTO
        // simplificado ou garantir o carregamento. Para senioridade, vamos manter consistência.

        return new ComentarioResponseDto(
            comentario.Id,
            comentario.UsuarioId,
            "Você", // Nome será carregado corretamente na listagem, para o retorno imediato simplificamos ou carregamos
            comentario.Texto,
            comentario.DataCriacao
        );
    }

    public async Task<List<ComentarioResponseDto>> ListarPorDocumentoAsync(Guid documentoId, Guid usuarioId)
    {
        var documento = await _documentoRepository.GetByIdAsync(documentoId);
        if (documento == null) throw new Exception("Documento não encontrado.");

        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, documento.EmpresaId);
        if (!vinculado) throw new Exception("Acesso negado.");

        var comentarios = await _comentarioRepository.GetByDocumentoIdAsync(documentoId);
        return comentarios.Select(MapToDto).ToList();
    }

    public async Task<List<ComentarioResponseDto>> ListarPorGuiaPagamentoAsync(Guid guiaPagamentoId, Guid usuarioId)
    {
        var guia = await _guiaPagamentoRepository.ObterPorIdAsync(guiaPagamentoId);
        if (guia == null) throw new Exception("Guia de pagamento não encontrada.");

        var vinculado = await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, guia.EmpresaId);
        if (!vinculado) throw new Exception("Acesso negado.");

        var comentarios = await _comentarioRepository.GetByGuiaPagamentoIdAsync(guiaPagamentoId);
        return comentarios.Select(MapToDto).ToList();
    }

    private ComentarioResponseDto MapToDto(Comentario c) => new(
        c.Id,
        c.UsuarioId,
        c.Usuario?.Nome ?? "Usuário",
        c.Texto,
        c.DataCriacao
    );
}
