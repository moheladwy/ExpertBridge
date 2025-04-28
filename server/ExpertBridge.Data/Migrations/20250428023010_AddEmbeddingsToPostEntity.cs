using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingsToPostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Vector>(
                name: "Embeddings",
                table: "Posts",
                type: "vector(1024)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embeddings",
                table: "Posts");
        }
    }
}
