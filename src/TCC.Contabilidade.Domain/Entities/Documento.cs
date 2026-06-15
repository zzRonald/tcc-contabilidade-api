using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Domain.Entities;

public class Documento : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid CompetenciaId { get; set; }
    public Guid? SolicitacaoDocumentoId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string Extensao { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public StatusDocumento Status { get; set; }
    public Guid? AnalisadoPorId { get; set; }
    public DateTime? DataAnalise { get; set; }
    public string? MotivoRejeicao { get; set; }

    // Propriedades de navegação
    public Empresa Empresa { get; set; } = null!;
    public Competencia Competencia { get; set; } = null!;
    public SolicitacaoDocumento? SolicitacaoDocumento { get; set; }
    public User Usuario { get; set; } = null!;

    public Documento()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Status = StatusDocumento.Pendente;
    }
}
