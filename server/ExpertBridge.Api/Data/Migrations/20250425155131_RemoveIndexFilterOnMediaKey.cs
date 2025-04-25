using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndexFilterOnMediaKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
