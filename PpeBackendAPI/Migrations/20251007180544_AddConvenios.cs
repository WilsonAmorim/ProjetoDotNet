using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PpeBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConvenios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Convenios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    convenio = table.Column<string>(type: "TEXT", nullable: true),
                    cpf = table.Column<string>(type: "TEXT", nullable: true),
                    matricula = table.Column<string>(type: "TEXT", nullable: true),
                    nome = table.Column<string>(type: "TEXT", nullable: true),
                    situacao = table.Column<string>(type: "TEXT", nullable: true),
                    categoria = table.Column<string>(type: "TEXT", nullable: true),
                    data_admissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    data_demissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    sexo = table.Column<int>(type: "INTEGER", nullable: false),
                    funcao = table.Column<string>(type: "TEXT", nullable: true),
                    data_atualizacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    usuario = table.Column<int>(type: "INTEGER", nullable: false),
                    posto_trabalho = table.Column<string>(type: "TEXT", nullable: true),
                    Municipio_lotacao = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convenios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Convenios");
        }
    }
}
