using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TCC.Contabilidade.Application.DTO.Relatorios;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        // QuestPDF license is set in Program.cs
    }

    public byte[] GenerateMonthlyReportPdf(RelatorioMensalDTO relatorio)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Relatório Mensal de Conformidade").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"{relatorio.NomeEmpresa}").FontSize(14).Medium();
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Competência: {relatorio.Mes:D2}/{relatorio.Ano}").FontSize(12).SemiBold();
                        col.Item().Text($"Status: {relatorio.StatusCompetencia}").FontSize(10).Italic();
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(15);

                    // Seção Documentos
                    column.Item().Element(c => ComposeSectionHeader(c, "Documentos"));
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Solicitados");
                            header.Cell().Element(CellStyle).Text("Enviados");
                            header.Cell().Element(CellStyle).Text("Aprovados");
                            header.Cell().Element(CellStyle).Text("Rejeitados");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        table.Cell().Element(CellValueStyle).Text(relatorio.Documentos.Solicitados.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Documentos.Enviados.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Documentos.Aprovados.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Documentos.Rejeitados.ToString());

                        static IContainer CellValueStyle(IContainer container)
                        {
                            return container.PaddingVertical(5);
                        }
                    });

                    // Seção Obrigações
                    column.Item().Element(c => ComposeSectionHeader(c, "Obrigações"));
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Pendentes");
                            header.Cell().Element(CellStyle).Text("Concluídas");
                            header.Cell().Element(CellStyle).Text("Atrasadas");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        table.Cell().Element(CellValueStyle).Text(relatorio.Obrigacoes.Pendentes.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Obrigacoes.Concluidas.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Obrigacoes.Atrasadas.ToString());

                        static IContainer CellValueStyle(IContainer container)
                        {
                            return container.PaddingVertical(5);
                        }
                    });

                    // Seção Guias de Pagamento
                    column.Item().Element(c => ComposeSectionHeader(c, "Guias de Pagamento"));
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Abertas");
                            header.Cell().Element(CellStyle).Text("Pagas");
                            header.Cell().Element(CellStyle).Text("Vencidas");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        table.Cell().Element(CellValueStyle).Text(relatorio.Guias.Abertas.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Guias.Pagas.ToString());
                        table.Cell().Element(CellValueStyle).Text(relatorio.Guias.Vencidas.ToString());

                        static IContainer CellValueStyle(IContainer container)
                        {
                            return container.PaddingVertical(5);
                        }
                    });

                    column.Item().PaddingTop(20).Text(x =>
                    {
                        x.Span("Nota: ").SemiBold();
                        x.Span("Este relatório é um resumo automático da situação da empresa na competência informada. Para mais detalhes, acesse a plataforma.");
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Gerado em: ").FontSize(9);
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9);
                    x.Span(" - Página ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9);
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeSectionHeader(IContainer container, string title)
    {
        container.PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(title).FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
    }
}
