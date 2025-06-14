// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FutherModificationsForHiringFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Jobs",
                type: "timestamp with time zone",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "CreatedAt",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProfileId1",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Jobs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
