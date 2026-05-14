using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omotemachi.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesUrlsToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Logs");

            migrationBuilder.AddColumn<string[]>(
                name: "FilesUrls",
                table: "Logs",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "ImagesUrls",
                table: "Logs",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilesUrls",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ImagesUrls",
                table: "Logs");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Logs",
                type: "bigint",
                nullable: true);
        }
    }
}
