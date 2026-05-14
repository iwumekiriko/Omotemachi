using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omotemachi.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddImageSaverFieldToChannelsConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ImageSaverChannelId",
                table: "ChannelsConfig",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSaverChannelId",
                table: "ChannelsConfig");
        }
    }
}
