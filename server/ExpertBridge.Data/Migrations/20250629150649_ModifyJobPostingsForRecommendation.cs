using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyJobPostingsForRecommendation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Areas_AreaId",
                table: "JobPostings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_JobCategories_CategoryId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Embedding",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobPostingId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "JobPostings");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "JobPostings",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Profiles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "JobPostings",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "JobPostings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "JobPostings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "JobPostings",
                type: "vector(1024)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "JobPostings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTagged",
                table: "JobPostings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobCategoryId",
                table: "JobPostings",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "JobPostings",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostId",
                table: "Comments",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AddColumn<string>(
                name: "JobPostingId",
                table: "Comments",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobPostingTags",
                columns: table => new
                {
                    JobPostingId = table.Column<string>(type: "character varying(450)", nullable: false),
                    TagId = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostingTags", x => new { x.JobPostingId, x.TagId });
                    table.ForeignKey(
                        name: "FK_JobPostingTags_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPostingTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPostingVotes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IsUpvote = table.Column<bool>(type: "boolean", nullable: false),
                    ProfileId = table.Column<string>(type: "character varying(450)", nullable: false),
                    JobPostingId = table.Column<string>(type: "character varying(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostingVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostingVotes_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPostingVotes_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Embedding",
                table: "Posts",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 128)
                .Annotation("Npgsql:StorageParameter:m", 64);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobPostingId",
                table: "Jobs",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Embedding",
                table: "JobPostings",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 128)
                .Annotation("Npgsql:StorageParameter:m", 64);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_JobCategoryId",
                table: "JobPostings",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_JobPostingId",
                table: "Comments",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingTags_TagId",
                table: "JobPostingTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingVotes_JobPostingId",
                table: "JobPostingVotes",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingVotes_ProfileId_JobPostingId",
                table: "JobPostingVotes",
                columns: new[] { "ProfileId", "JobPostingId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_JobPostings_JobPostingId",
                table: "Comments",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Areas_AreaId",
                table: "JobPostings",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_JobCategories_JobCategoryId",
                table: "JobPostings",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_JobPostings_JobPostingId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Areas_AreaId",
                table: "JobPostings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_JobCategories_JobCategoryId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "JobPostingTags");

            migrationBuilder.DropTable(
                name: "JobPostingVotes");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Embedding",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobPostingId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Embedding",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_JobCategoryId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_Comments_JobPostingId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsTagged",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobCategoryId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobPostingId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "JobPostings",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Profiles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "JobPostings",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "JobPostings",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "JobPostings",
                type: "double precision",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "PostId",
                table: "Comments",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Embedding",
                table: "Posts",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobPostingId",
                table: "Jobs",
                column: "JobPostingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Areas_AreaId",
                table: "JobPostings",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_JobCategories_CategoryId",
                table: "JobPostings",
                column: "CategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
