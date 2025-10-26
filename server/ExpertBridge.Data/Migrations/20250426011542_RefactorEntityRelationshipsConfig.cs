// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEntityRelationshipsConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "ProfileMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileExperienceId",
                table: "ProfileExperienceMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PostId",
                table: "PostMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "JobPostingId",
                table: "JobPostingMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CommentId",
                table: "CommentMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "ChatMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "ProfileMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileExperienceId",
                table: "ProfileExperienceMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostId",
                table: "PostMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobPostingId",
                table: "JobPostingMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommentId",
                table: "CommentMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "ChatMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);
        }
    }
}
