using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaGrantModelConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MediaGrants_OnHold_GrantedAt",
                table: "MediaGrants",
                columns: new[] { "OnHold", "GrantedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaGrants_OnHold_GrantedAt",
                table: "MediaGrants");
        }
    }
}
