using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCC.Contabilidade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoAnalysisFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnalisadoPorId",
                table: "Documentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAnalise",
                table: "Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRejeicao",
                table: "Documentos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalisadoPorId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "DataAnalise",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "MotivoRejeicao",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documentos");
        }
    }
}
