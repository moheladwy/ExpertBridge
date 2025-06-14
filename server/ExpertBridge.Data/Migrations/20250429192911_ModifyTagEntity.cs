// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Api.ExpertBridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTagEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tags",
                newName: "EnglishName");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                newName: "IX_Tags_EnglishName");

            migrationBuilder.RenameColumn(
                name: "Embeddings",
                table: "Posts",
                newName: "Embedding");

            migrationBuilder.AddColumn<string>(
                name: "ArabicName",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ArabicName",
                table: "Tags",
                column: "ArabicName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_ArabicName",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ArabicName",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "EnglishName",
                table: "Tags",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_EnglishName",
                table: "Tags",
                newName: "IX_Tags_Name");

            migrationBuilder.RenameColumn(
                name: "Embedding",
                table: "Posts",
                newName: "Embeddings");
        }
    }
}
