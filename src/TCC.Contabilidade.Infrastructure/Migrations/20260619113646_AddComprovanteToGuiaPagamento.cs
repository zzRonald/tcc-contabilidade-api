using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCC.Contabilidade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComprovanteToGuiaPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ComprovanteId",
                table: "GuiasPagamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuiasPagamento_ComprovanteId",
                table: "GuiasPagamento",
                column: "ComprovanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuiasPagamento_Documentos_ComprovanteId",
                table: "GuiasPagamento",
                column: "ComprovanteId",
                principalTable: "Documentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuiasPagamento_Documentos_ComprovanteId",
                table: "GuiasPagamento");

            migrationBuilder.DropIndex(
                name: "IX_GuiasPagamento_ComprovanteId",
                table: "GuiasPagamento");

            migrationBuilder.DropColumn(
                name: "ComprovanteId",
                table: "GuiasPagamento");
        }
    }
}
