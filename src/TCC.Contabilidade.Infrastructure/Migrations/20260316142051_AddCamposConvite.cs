using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCC.Contabilidade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposConvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataExpiracao",
                table: "Convites",
                newName: "Expiracao");

            migrationBuilder.AddColumn<bool>(
                name: "Usado",
                table: "Convites",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usado",
                table: "Convites");

            migrationBuilder.RenameColumn(
                name: "Expiracao",
                table: "Convites",
                newName: "DataExpiracao");
        }
    }
}
