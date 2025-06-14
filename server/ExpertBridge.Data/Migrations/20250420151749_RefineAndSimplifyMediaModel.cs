// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefineAndSimplifyMediaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_MediaTypes_MediaTypeId",
                table: "Media");

            migrationBuilder.DropTable(
                name: "ChatMedias");

            migrationBuilder.DropTable(
                name: "CommentMedias");

            migrationBuilder.DropTable(
                name: "JobPostingMedias");

            migrationBuilder.DropTable(
                name: "PostMedias");

            migrationBuilder.DropTable(
                name: "ProfileExperienceMedias");

            migrationBuilder.DropTable(
                name: "ProfileMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Media",
                table: "Media");

            migrationBuilder.RenameTable(
                name: "Media",
                newName: "MediaObject");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "MediaObject",
                newName: "Url");

            migrationBuilder.RenameIndex(
                name: "IX_Media_MediaUrl",
                table: "MediaObject",
                newName: "IX_MediaObject_Url");

            migrationBuilder.RenameIndex(
                name: "IX_Media_MediaTypeId",
                table: "MediaObject",
                newName: "IX_MediaObject_MediaTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "MediaTypeId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "MediaObject",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobPostingId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "MediaObject",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileExperienceId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "MediaObject",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MediaObject",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaObject",
                table: "MediaObject",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_ChatId",
                table: "MediaObject",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_CommentId",
                table: "MediaObject",
                column: "CommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_JobPostingId",
                table: "MediaObject",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_PostId",
                table: "MediaObject",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_ProfileExperienceId",
                table: "MediaObject",
                column: "ProfileExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaObject_ProfileId",
                table: "MediaObject",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_Chats_ChatId",
                table: "MediaObject",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_Comments_CommentId",
                table: "MediaObject",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_JobPostings_JobPostingId",
                table: "MediaObject",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_MediaTypes_MediaTypeId",
                table: "MediaObject",
                column: "MediaTypeId",
                principalTable: "MediaTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_Posts_PostId",
                table: "MediaObject",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_ProfileExperiences_ProfileExperienceId",
                table: "MediaObject",
                column: "ProfileExperienceId",
                principalTable: "ProfileExperiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaObject_Profiles_ProfileId",
                table: "MediaObject",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_Chats_ChatId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_Comments_CommentId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_JobPostings_JobPostingId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_MediaTypes_MediaTypeId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_Posts_PostId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_ProfileExperiences_ProfileExperienceId",
                table: "MediaObject");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaObject_Profiles_ProfileId",
                table: "MediaObject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaObject",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_ChatId",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_CommentId",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_JobPostingId",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_PostId",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_ProfileExperienceId",
                table: "MediaObject");

            migrationBuilder.DropIndex(
                name: "IX_MediaObject_ProfileId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "JobPostingId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "ProfileExperienceId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "MediaObject");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MediaObject");

            migrationBuilder.RenameTable(
                name: "MediaObject",
                newName: "Media");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Media",
                newName: "MediaUrl");

            migrationBuilder.RenameIndex(
                name: "IX_MediaObject_Url",
                table: "Media",
                newName: "IX_Media_MediaUrl");

            migrationBuilder.RenameIndex(
                name: "IX_MediaObject_MediaTypeId",
                table: "Media",
                newName: "IX_Media_MediaTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "MediaTypeId",
                table: "Media",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Media",
                table: "Media",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChatMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ChatId = table.Column<string>(type: "character varying(450)", nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMedias_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CommentId = table.Column<string>(type: "character varying(450)", nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentMedias_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPostingMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    JobPostingId = table.Column<string>(type: "character varying(450)", nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostingMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostingMedias_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPostingMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false),
                    PostId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostMedias_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileExperienceMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false),
                    ProfileExperienceId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileExperienceMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileExperienceMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileExperienceMedias_ProfileExperiences_ProfileExperienc~",
                        column: x => x.ProfileExperienceId,
                        principalTable: "ProfileExperiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    MediaId = table.Column<string>(type: "character varying(450)", nullable: false),
                    ProfileId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileMedias_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileMedias_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMedias_ChatId",
                table: "ChatMedias",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMedias_MediaId",
                table: "ChatMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias",
                column: "CommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_MediaId",
                table: "CommentMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingMedias_JobPostingId",
                table: "JobPostingMedias",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingMedias_MediaId",
                table: "JobPostingMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_MediaId",
                table: "PostMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_PostId",
                table: "PostMedias",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileExperienceMedias_MediaId",
                table: "ProfileExperienceMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileExperienceMedias_ProfileExperienceId",
                table: "ProfileExperienceMedias",
                column: "ProfileExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileMedias_MediaId",
                table: "ProfileMedias",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileMedias_ProfileId",
                table: "ProfileMedias",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_MediaTypes_MediaTypeId",
                table: "Media",
                column: "MediaTypeId",
                principalTable: "MediaTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
