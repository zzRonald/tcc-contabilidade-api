using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCC.Contabilidade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitacaoDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitacoesDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ObservacaoContador = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesDocumentos_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesDocumentos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesDocumentos_CompetenciaId",
                table: "SolicitacoesDocumentos",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesDocumentos_EmpresaId",
                table: "SolicitacoesDocumentos",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacoesDocumentos");
        }
    }
}
