using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Jobs",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "JobOffers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeclined",
                table: "JobOffers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HirerId",
                table: "Chats",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkerId",
                table: "Chats",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "ChatMedias",
                type: "character varying(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    SenderId = table.Column<string>(type: "character varying(450)", nullable: false),
                    ChatId = table.Column<string>(type: "character varying(450)", nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsConfirmationMessage = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Profiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ChatId",
                table: "Jobs",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_HirerId",
                table: "Chats",
                column: "HirerId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_WorkerId",
                table: "Chats",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Profiles_HirerId",
                table: "Chats",
                column: "HirerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Profiles_WorkerId",
                table: "Chats",
                column: "WorkerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Chats_ChatId",
                table: "Jobs",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Profiles_HirerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Profiles_WorkerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Chats_ChatId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ChatId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Chats_HirerId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_WorkerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "IsDeclined",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "HirerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "ChatId",
                table: "ChatMedias",
                type: "character varying(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)");
        }
    }
}
