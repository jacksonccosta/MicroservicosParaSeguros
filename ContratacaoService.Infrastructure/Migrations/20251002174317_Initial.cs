using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContratacaoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratacoes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratacoes_PropostaId",
                table: "Contratacoes",
                column: "PropostaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratacoes");
        }
    }
}
