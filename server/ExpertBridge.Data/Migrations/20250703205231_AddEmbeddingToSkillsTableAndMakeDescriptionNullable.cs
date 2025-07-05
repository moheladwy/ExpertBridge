using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingToSkillsTableAndMakeDescriptionNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Skills",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Skills",
                type: "vector(1024)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Embedding",
                table: "Skills",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 128)
                .Annotation("Npgsql:StorageParameter:m", 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skills_Embedding",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Skills");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Skills",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
