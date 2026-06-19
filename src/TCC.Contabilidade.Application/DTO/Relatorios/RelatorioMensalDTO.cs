namespace TCC.Contabilidade.Application.DTO.Relatorios;

public class RelatorioMensalDTO
{
    public Guid EmpresaId { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string StatusCompetencia { get; set; } = string.Empty;

    public DocumentoStatsDTO Documentos { get; set; } = new();
    public ObrigacaoStatsDTO Obrigacoes { get; set; } = new();
    public GuiaPagamentoStatsDTO Guias { get; set; } = new();
}

public class DocumentoStatsDTO
{
    public int Solicitados { get; set; }
    public int Enviados { get; set; }
    public int Aprovados { get; set; }
    public int Rejeitados { get; set; }
}

public class ObrigacaoStatsDTO
{
    public int Pendentes { get; set; }
    public int Concluidas { get; set; }
    public int Atrasadas { get; set; }
}

public class GuiaPagamentoStatsDTO
{
    public int Abertas { get; set; }
    public int Pagas { get; set; }
    public int Vencidas { get; set; }
}
