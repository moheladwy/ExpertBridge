using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProcessedPropToCommentAndIndexOnIt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsProcessed",
                table: "Posts",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsTagged",
                table: "Posts",
                column: "IsTagged");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsProcessed",
                table: "Comments",
                column: "IsProcessed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_IsProcessed",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_IsTagged",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_IsProcessed",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Comments");
        }
    }
}
