namespace TCC.Contabilidade.Application.DTO;

public record CreateComentarioDto(
    Guid? DocumentoId,
    Guid? GuiaPagamentoId,
    string Texto
);

public record ComentarioResponseDto(
    Guid Id,
    Guid UsuarioId,
    string NomeUsuario,
    string Texto,
    DateTime DataCriacao
);
