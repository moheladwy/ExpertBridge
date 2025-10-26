// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCommentToHaveManyMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias",
                column: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias",
                column: "CommentId",
                unique: true);
        }
    }
}
