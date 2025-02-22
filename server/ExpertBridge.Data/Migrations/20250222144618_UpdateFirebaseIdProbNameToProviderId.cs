using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFirebaseIdProbNameToProviderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirebaseId",
                table: "Users",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_FirebaseId",
                table: "Users",
                newName: "IX_Users_ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Users",
                newName: "FirebaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ProviderId",
                table: "Users",
                newName: "IX_Users_FirebaseId");
        }
    }
}
