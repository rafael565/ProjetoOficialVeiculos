using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoOficialVeiculos.Migrations
{
    /// <inheritdoc />
    public partial class Inical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caminhoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    placa = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caminhoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    cnpj = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    contato = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    endereco = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Materiais",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    quantidade = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Motoristas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motoristas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caminhaoID = table.Column<int>(type: "int", nullable: false),
                    empresaID = table.Column<int>(type: "int", nullable: false),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    DataAgendamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PesoCarregado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrdemFila = table.Column<int>(type: "int", nullable: false),
                    MotoristaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Caminhoes_caminhaoID",
                        column: x => x.caminhaoID,
                        principalTable: "Caminhoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Empresas_empresaID",
                        column: x => x.empresaID,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Materiais_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "Materiais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Motoristas_MotoristaID",
                        column: x => x.MotoristaID,
                        principalTable: "Motoristas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_caminhaoID",
                table: "Agendamentos",
                column: "caminhaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_empresaID",
                table: "Agendamentos",
                column: "empresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_MaterialID",
                table: "Agendamentos",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_MotoristaID",
                table: "Agendamentos",
                column: "MotoristaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Caminhoes");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Materiais");

            migrationBuilder.DropTable(
                name: "Motoristas");
        }
    }
}
