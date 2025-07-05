using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobOfferEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Profiles_ProfileId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Profiles_ProfileId1",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ProfileId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ProfileId1",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProfileId1",
                table: "Jobs");

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualCost",
                table: "Jobs",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReviewId",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobOffers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuthorId = table.Column<string>(type: "character varying(450)", nullable: false),
                    WorkerId = table.Column<string>(type: "character varying(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOffers_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobOffers_Profiles_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobOffers_AuthorId",
                table: "JobOffers",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffers_WorkerId",
                table: "JobOffers",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobOffers");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Jobs");

            migrationBuilder.AlterColumn<double>(
                name: "ActualCost",
                table: "Jobs",
                type: "double precision",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "Jobs",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileId1",
                table: "Jobs",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ProfileId",
                table: "Jobs",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ProfileId1",
                table: "Jobs",
                column: "ProfileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Profiles_ProfileId",
                table: "Jobs",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Profiles_ProfileId1",
                table: "Jobs",
                column: "ProfileId1",
                principalTable: "Profiles",
                principalColumn: "Id");
        }
    }
}
