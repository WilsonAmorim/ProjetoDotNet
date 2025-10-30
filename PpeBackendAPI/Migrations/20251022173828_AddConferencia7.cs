using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PpeBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConferencia7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistroOcorrenciasId",
                table: "Conferencias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RegistroOcorrencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroOcorrencias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conferencias_RegistroOcorrenciasId",
                table: "Conferencias",
                column: "RegistroOcorrenciasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conferencias_RegistroOcorrencias_RegistroOcorrenciasId",
                table: "Conferencias",
                column: "RegistroOcorrenciasId",
                principalTable: "RegistroOcorrencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conferencias_RegistroOcorrencias_RegistroOcorrenciasId",
                table: "Conferencias");

            migrationBuilder.DropTable(
                name: "RegistroOcorrencias");

            migrationBuilder.DropIndex(
                name: "IX_Conferencias_RegistroOcorrenciasId",
                table: "Conferencias");

            migrationBuilder.DropColumn(
                name: "RegistroOcorrenciasId",
                table: "Conferencias");
        }
    }
}
