using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnBoarded",
                table: "Users",
                newName: "IsOnboarded");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Posts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Comments",
                newName: "IsDeleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTagged",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsTagged",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "IsOnboarded",
                table: "Users",
                newName: "IsOnBoarded");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Posts",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Comments",
                newName: "isDeleted");
        }
    }
}
