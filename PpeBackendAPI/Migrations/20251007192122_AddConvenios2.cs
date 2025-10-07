using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PpeBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConvenios2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usuario",
                table: "Convenios",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "situacao",
                table: "Convenios",
                newName: "Situacao");

            migrationBuilder.RenameColumn(
                name: "sexo",
                table: "Convenios",
                newName: "Sexo");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Convenios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "matricula",
                table: "Convenios",
                newName: "Matricula");

            migrationBuilder.RenameColumn(
                name: "funcao",
                table: "Convenios",
                newName: "Funcao");

            migrationBuilder.RenameColumn(
                name: "cpf",
                table: "Convenios",
                newName: "Cpf");

            migrationBuilder.RenameColumn(
                name: "categoria",
                table: "Convenios",
                newName: "Categoria");

            migrationBuilder.RenameColumn(
                name: "posto_trabalho",
                table: "Convenios",
                newName: "PostoTrabalho");

            migrationBuilder.RenameColumn(
                name: "data_demissao",
                table: "Convenios",
                newName: "DataDemissao");

            migrationBuilder.RenameColumn(
                name: "data_atualizacao",
                table: "Convenios",
                newName: "DataAtualizacao");

            migrationBuilder.RenameColumn(
                name: "data_admissao",
                table: "Convenios",
                newName: "DataAdmissao");

            migrationBuilder.RenameColumn(
                name: "convenio",
                table: "Convenios",
                newName: "MunicipioLotacao");

            migrationBuilder.RenameColumn(
                name: "Municipio_lotacao",
                table: "Convenios",
                newName: "ConvenioNome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Usuario",
                table: "Convenios",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "Situacao",
                table: "Convenios",
                newName: "situacao");

            migrationBuilder.RenameColumn(
                name: "Sexo",
                table: "Convenios",
                newName: "sexo");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Convenios",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Matricula",
                table: "Convenios",
                newName: "matricula");

            migrationBuilder.RenameColumn(
                name: "Funcao",
                table: "Convenios",
                newName: "funcao");

            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "Convenios",
                newName: "cpf");

            migrationBuilder.RenameColumn(
                name: "Categoria",
                table: "Convenios",
                newName: "categoria");

            migrationBuilder.RenameColumn(
                name: "PostoTrabalho",
                table: "Convenios",
                newName: "posto_trabalho");

            migrationBuilder.RenameColumn(
                name: "MunicipioLotacao",
                table: "Convenios",
                newName: "convenio");

            migrationBuilder.RenameColumn(
                name: "DataDemissao",
                table: "Convenios",
                newName: "data_demissao");

            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                table: "Convenios",
                newName: "data_atualizacao");

            migrationBuilder.RenameColumn(
                name: "DataAdmissao",
                table: "Convenios",
                newName: "data_admissao");

            migrationBuilder.RenameColumn(
                name: "ConvenioNome",
                table: "Convenios",
                newName: "Municipio_lotacao");
        }
    }
}
