using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FurtherEnhanceJobApplicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Profiles_ContractorProfileId",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "ContractorProfileId",
                table: "JobApplications",
                newName: "ApplicantId");

            migrationBuilder.RenameColumn(
                name: "AppliedAt",
                table: "JobApplications",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_ContractorProfileId",
                table: "JobApplications",
                newName: "IX_JobApplications_ApplicantId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobPostingId",
                table: "JobApplications",
                type: "character varying(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CoverLetter",
                table: "JobApplications",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "JobApplications",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobApplications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "JobApplications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OfferedCost",
                table: "JobApplications",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobPostingId",
                table: "JobApplications",
                column: "JobPostingId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobPostings_JobPostingId",
                table: "JobApplications",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Profiles_ApplicantId",
                table: "JobApplications",
                column: "ApplicantId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobPostings_JobPostingId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Profiles_ApplicantId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_JobPostingId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "OfferedCost",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "JobApplications",
                newName: "AppliedAt");

            migrationBuilder.RenameColumn(
                name: "ApplicantId",
                table: "JobApplications",
                newName: "ContractorProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_ApplicantId",
                table: "JobApplications",
                newName: "IX_JobApplications_ContractorProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "JobPostingId",
                table: "JobApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CoverLetter",
                table: "JobApplications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "JobApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Profiles_ContractorProfileId",
                table: "JobApplications",
                column: "ContractorProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
