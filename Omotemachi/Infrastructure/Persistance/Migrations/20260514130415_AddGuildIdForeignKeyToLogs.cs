using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omotemachi.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildIdForeignKeyToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Logs_GuildId",
                table: "Logs",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Guilds_GuildId",
                table: "Logs",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Guilds_GuildId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_GuildId",
                table: "Logs");
        }
    }
}
