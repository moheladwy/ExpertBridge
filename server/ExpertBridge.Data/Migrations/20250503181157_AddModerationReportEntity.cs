using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModerationReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    ContentId = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    IsNegative = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Toxicity = table.Column<double>(type: "double precision", nullable: false),
                    SevereToxicity = table.Column<double>(type: "double precision", nullable: false),
                    Obscene = table.Column<double>(type: "double precision", nullable: false),
                    Threat = table.Column<double>(type: "double precision", nullable: false),
                    Insult = table.Column<double>(type: "double precision", nullable: false),
                    IdentityAttack = table.Column<double>(type: "double precision", nullable: false),
                    SexualExplicit = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_ContentId",
                table: "ModerationReports",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_IsNegative",
                table: "ModerationReports",
                column: "IsNegative");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReports_IsNegative_ContentId",
                table: "ModerationReports",
                columns: new[] { "IsNegative", "ContentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModerationReports");
        }
    }
}
