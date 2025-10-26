// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseModelToOtherEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProfileExperiences",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProfileExperiences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProfileExperiences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "ProfileExperiences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobPostings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobPostings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "JobPostings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProfileExperiences");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProfileExperiences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProfileExperiences");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "ProfileExperiences");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Chats");
        }
    }
}
