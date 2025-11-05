using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PpeBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class dashboard2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estagios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DescricaoEstagio = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: true),
                    Periodo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estagios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Investimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DescricaoInvestimento = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: true),
                    Periodo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeLote = table.Column<string>(type: "TEXT", nullable: true),
                    ValorRepasse = table.Column<string>(type: "TEXT", nullable: true),
                    Periodo = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Delay = table.Column<string>(type: "TEXT", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorInformado = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repasses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estagios");

            migrationBuilder.DropTable(
                name: "Investimentos");

            migrationBuilder.DropTable(
                name: "Repasses");
        }
    }
}
