using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGinIndexForFirstAndLastNamesInProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FirstName_LastName",
                table: "Profiles",
                columns: new[] { "FirstName", "LastName" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profiles_FirstName_LastName",
                table: "Profiles");
        }
    }
}
