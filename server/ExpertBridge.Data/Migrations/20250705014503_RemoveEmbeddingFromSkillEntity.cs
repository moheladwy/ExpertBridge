using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmbeddingFromSkillEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skills_Embedding",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Skills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
