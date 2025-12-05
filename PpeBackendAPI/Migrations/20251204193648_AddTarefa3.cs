using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PpeBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTarefa3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TarefaId",
                table: "Conferencias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TarefaId",
                table: "Conferencias");
        }
    }
}
