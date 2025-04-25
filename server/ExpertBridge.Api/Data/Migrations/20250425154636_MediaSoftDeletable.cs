using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class MediaSoftDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProviderId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ProfileMedias_Key",
                table: "ProfileMedias");

            migrationBuilder.DropIndex(
                name: "IX_ProfileExperienceMedias_Key",
                table: "ProfileExperienceMedias");

            migrationBuilder.DropIndex(
                name: "IX_PostMedias_Key",
                table: "PostMedias");

            migrationBuilder.DropIndex(
                name: "IX_JobPostingMedias_Key",
                table: "JobPostingMedias");

            migrationBuilder.DropIndex(
                name: "IX_CommentMedias_Key",
                table: "CommentMedias");

            migrationBuilder.DropIndex(
                name: "IX_ChatMedias_Key",
                table: "ChatMedias");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProfileMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProfileMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProfileExperienceMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProfileExperienceMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PostMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PostMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobPostingMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobPostingMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CommentMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CommentMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ChatMedias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatMedias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProviderId",
                table: "Users",
                column: "ProviderId",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileMedias_Key",
                table: "ProfileMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileExperienceMedias_Key",
                table: "ProfileExperienceMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_Key",
                table: "PostMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingMedias_Key",
                table: "JobPostingMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_Key",
                table: "CommentMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMedias_Key",
                table: "ChatMedias",
                column: "Key",
                unique: true,
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProviderId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ProfileMedias_Key",
                table: "ProfileMedias");

            migrationBuilder.DropIndex(
                name: "IX_ProfileExperienceMedias_Key",
                table: "ProfileExperienceMedias");

            migrationBuilder.DropIndex(
                name: "IX_PostMedias_Key",
                table: "PostMedias");

            migrationBuilder.DropIndex(
                name: "IX_JobPostingMedias_Key",
                table: "JobPostingMedias");

            migrationBuilder.DropIndex(
                name: "IX_CommentMedias_Key",
                table: "CommentMedias");

            migrationBuilder.DropIndex(
                name: "IX_ChatMedias_Key",
                table: "ChatMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProfileMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProfileMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProfileExperienceMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProfileExperienceMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PostMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PostMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobPostingMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobPostingMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CommentMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CommentMedias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ChatMedias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatMedias");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProviderId",
                table: "Users",
                column: "ProviderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileMedias_Key",
                table: "ProfileMedias",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileExperienceMedias_Key",
                table: "ProfileExperienceMedias",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_Key",
                table: "PostMedias",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingMedias_Key",
                table: "JobPostingMedias",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_Key",
                table: "CommentMedias",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMedias_Key",
                table: "ChatMedias",
                column: "Key",
                unique: true);
        }
    }
}
