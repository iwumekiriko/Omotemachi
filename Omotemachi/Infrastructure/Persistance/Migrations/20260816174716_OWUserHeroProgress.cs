using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omotemachi.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class OWUserHeroProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserHeroProgresses",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Progress = table.Column<string>(type: "jsonb", nullable: false),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHeroProgresses", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserHeroProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserHeroProgresses");
        }
    }
}
