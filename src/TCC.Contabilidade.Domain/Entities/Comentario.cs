using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class Comentario : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? DocumentoId { get; set; }
    public Guid? GuiaPagamentoId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }

    // Navigation properties
    public Empresa Empresa { get; set; } = null!;
    public User Usuario { get; set; } = null!;
    public Documento? Documento { get; set; }
    public GuiaPagamento? GuiaPagamento { get; set; }

    public Comentario()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
    }
}
