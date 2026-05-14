using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Omotemachi.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AssetUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InteractionsAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetUrl = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionsAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Aliases = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelsConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    GeneralChannelId = table.Column<long>(type: "bigint", nullable: true),
                    OfftopChannelId = table.Column<long>(type: "bigint", nullable: true),
                    NitroBoostingChannelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelsConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_ChannelsConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EconomyConfigs",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    DefaultCurrencyIcon = table.Column<string>(type: "text", nullable: false),
                    DonateCurrencyIcon = table.Column<string>(type: "text", nullable: false),
                    DailyBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomyConfigs", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_EconomyConfigs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpBoosters",
                columns: table => new
                {
                    ExpBoosterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpBoosters", x => x.ExpBoosterId);
                    table.ForeignKey(
                        name: "FK_ExpBoosters_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    ExpForMessage = table.Column<int>(type: "integer", nullable: true),
                    ExpForVoiceMinute = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_ExperienceConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogsConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    CommandInteractionsWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    MessagesWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    TicketsWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    GuildWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    MembersWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    VoiceWebhookUrl = table.Column<string>(type: "text", nullable: true),
                    ElseWebhookUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_LogsConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootboxesConfigs",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    RolesLootboxKeyPrice = table.Column<int>(type: "integer", nullable: true),
                    BackgroundsLootboxKeyPrice = table.Column<int>(type: "integer", nullable: true),
                    ActiveLootboxes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootboxesConfigs", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_LootboxesConfigs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootboxKeys",
                columns: table => new
                {
                    LootboxKeyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootboxKeys", x => x.LootboxKeyId);
                    table.ForeignKey(
                        name: "FK_LootboxKeys_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootboxRoles",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    GuildRoleId = table.Column<long>(type: "bigint", nullable: false),
                    LootboxType = table.Column<int>(type: "integer", nullable: false),
                    Exclusive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootboxRoles", x => new { x.GuildId, x.GuildRoleId });
                    table.ForeignKey(
                        name: "FK_LootboxRoles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PacksConfigs",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    PacksPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacksConfigs", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_PacksConfigs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    ChannelId = table.Column<long>(type: "bigint", nullable: true),
                    Required = table.Column<int>(type: "integer", nullable: false),
                    RewardType = table.Column<int>(type: "integer", nullable: false),
                    RewardAmount = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    IsAvailableNow = table.Column<bool>(type: "boolean", nullable: false),
                    CompletableUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quests_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestsConfigs",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    QuestsChannelId = table.Column<long>(type: "bigint", nullable: true),
                    QuestsMessageId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestsConfigs", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_QuestsConfigs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildRoleId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    SupportRoleId = table.Column<long>(type: "bigint", nullable: true),
                    ModeratorRoleId = table.Column<long>(type: "bigint", nullable: true),
                    DeveloperRoleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_RolesConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    ShopChannelId = table.Column<long>(type: "bigint", nullable: true),
                    ShopMessageId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_ShopConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopKeys",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    LootboxType = table.Column<int>(type: "integer", nullable: false),
                    Exclusive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopKeys", x => new { x.GuildId, x.LootboxType });
                    table.ForeignKey(
                        name: "FK_ShopKeys_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopRoles",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    GuildRoleId = table.Column<long>(type: "bigint", nullable: false),
                    Exclusive = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopRoles", x => new { x.GuildId, x.GuildRoleId });
                    table.ForeignKey(
                        name: "FK_ShopRoles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketsConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    TicketChannelId = table.Column<long>(type: "bigint", nullable: true),
                    TicketMessageId = table.Column<long>(type: "bigint", nullable: true),
                    TicketReportChannelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketsConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_TicketsConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceConfig",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    CustomVoiceCreationChannelId = table.Column<long>(type: "bigint", nullable: true),
                    CustomVoiceCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    CustomVoiceDeletionTime = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceConfig", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_VoiceConfig_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SeriesId = table.Column<int>(type: "integer", nullable: false),
                    AssetsUrls = table.Column<List<string>>(type: "text[]", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsSeriesSpecific = table.Column<bool>(type: "boolean", nullable: false),
                    SeriesId = table.Column<int>(type: "integer", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packs_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActiveExpBoosters",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    ActivatedAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveExpBoosters", x => new { x.GuildId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ActiveExpBoosters_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveExpBoosters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    PacksOpenedCount = table.Column<int>(type: "integer", nullable: false),
                    CardsSwappedToPackCount = table.Column<int>(type: "integer", nullable: false),
                    CardsTradedCount = table.Column<int>(type: "integer", nullable: false),
                    CardsGiftedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_CardStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DNDStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    DNDDiceRolledCount = table.Column<int>(type: "integer", nullable: false),
                    DNDDiceRolledMaxCount = table.Column<int>(type: "integer", nullable: false),
                    DNDDiceRolledMinCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DNDStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_DNDStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DNDStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Duets",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    ProposerId = table.Column<long>(type: "bigint", nullable: false),
                    DuoId = table.Column<long>(type: "bigint", nullable: false),
                    TogetherFrom = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duets", x => new { x.GuildId, x.ProposerId, x.DuoId });
                    table.ForeignKey(
                        name: "FK_Duets_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Duets_Users_DuoId",
                        column: x => x.DuoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Duets_Users_ProposerId",
                        column: x => x.ProposerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DuetsStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    DuetsCreatedCount = table.Column<int>(type: "integer", nullable: false),
                    DuetsDivorcedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuetsStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_DuetsStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DuetsStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventories_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    ExpBoostersActivatedCount = table.Column<int>(type: "integer", nullable: false),
                    ExpGainedWithBoosters = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_InventoryStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootboxesStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    RolesLootboxesOpenedCount = table.Column<int>(type: "integer", nullable: false),
                    BackgroundsLootboxesOpenedCount = table.Column<int>(type: "integer", nullable: false),
                    LootboxesRolesDroppedCount = table.Column<int>(type: "integer", nullable: false),
                    LootboxesBackgroundsDroppedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootboxesStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_LootboxesStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LootboxesStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootboxUserDatas",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LootboxType = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootboxUserDatas", x => new { x.GuildId, x.UserId, x.LootboxType });
                    table.ForeignKey(
                        name: "FK_LootboxUserDatas_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LootboxUserDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    ExpMultiplier = table.Column<int>(type: "integer", nullable: false),
                    Coins = table.Column<int>(type: "integer", nullable: false),
                    Crystals = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsBot = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => new { x.GuildId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Members_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Members_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembersStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    CoinsAmountChangedCount = table.Column<int>(type: "integer", nullable: false),
                    CrystalsAmountChangedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_MembersStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembersStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessagesStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    MessagesWritenCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_MessagesStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessagesStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestsStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    QuestsAssignedCount = table.Column<int>(type: "integer", nullable: false),
                    QuestsCompletedCount = table.Column<int>(type: "integer", nullable: false),
                    CoinsFromQuestsCount = table.Column<int>(type: "integer", nullable: false),
                    CrystallsFromQuestsCount = table.Column<int>(type: "integer", nullable: false),
                    LootboxKeysFromQuestsCount = table.Column<int>(type: "integer", nullable: false),
                    CardsPacksFromQuestsCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestsStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_QuestsStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestsStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopRolesTries",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildRoleId = table.Column<long>(type: "bigint", nullable: false),
                    TriesUsed = table.Column<int>(type: "integer", nullable: false),
                    TryActivated = table.Column<long>(type: "bigint", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopRolesTries", x => new { x.GuildId, x.UserId, x.GuildRoleId });
                    table.ForeignKey(
                        name: "FK_ShopRolesTries_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopRolesTries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateClose = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DescriptionProblem = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true),
                    Solution = table.Column<string>(type: "text", nullable: true),
                    TypeProblem = table.Column<string>(type: "text", nullable: true),
                    ModeratorId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketsStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    SupportTicketCreatedCount = table.Column<int>(type: "integer", nullable: false),
                    ModeratorTicketCreatedCount = table.Column<int>(type: "integer", nullable: false),
                    DeveloperTicketCreatedCount = table.Column<int>(type: "integer", nullable: false),
                    TicketsWasStartedCount = table.Column<int>(type: "integer", nullable: false),
                    TicketsWasClosedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketsStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_TicketsStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketsStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeoutAppaCatches",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LastCatch = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeoutAppaCatches", x => new { x.GuildId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TimeoutAppaCatches_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeoutAppaCatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeoutCardCatches",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LastGive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeoutCardCatches", x => new { x.GuildId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TimeoutCardCatches_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeoutCardCatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    PayerId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_PayerId",
                        column: x => x.PayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAppas",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppaId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    AcquiredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppas", x => new { x.GuildId, x.UserId, x.AppaId });
                    table.ForeignKey(
                        name: "FK_UserAppas_Appas_AppaId",
                        column: x => x.AppaId,
                        principalTable: "Appas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppas_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceStatistics",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    VoiceTimeMuted = table.Column<int>(type: "integer", nullable: false),
                    VoiceTimeUnMuted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceStatistics", x => new { x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_VoiceStatistics_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserQuests",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuests", x => new { x.QuestId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserQuests_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserQuests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SettingId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => new { x.SettingId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserSettings_Settings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCards",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    AssetIndex = table.Column<int>(type: "integer", nullable: false),
                    AcquiredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCards", x => new { x.CardId, x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_UserCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCards_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardPacks",
                columns: table => new
                {
                    CardsId = table.Column<int>(type: "integer", nullable: false),
                    PacksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPacks", x => new { x.CardsId, x.PacksId });
                    table.ForeignKey(
                        name: "FK_CardPacks_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardPacks_Packs_PacksId",
                        column: x => x.PacksId,
                        principalTable: "Packs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPacks",
                columns: table => new
                {
                    GuildId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PackId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPacks", x => new { x.PackId, x.UserId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_UserPacks_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPacks_Packs_PackId",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryExpBoosters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    ExpBoosterId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryExpBoosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryExpBoosters_ExpBoosters_ExpBoosterId",
                        column: x => x.ExpBoosterId,
                        principalTable: "ExpBoosters",
                        principalColumn: "ExpBoosterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryExpBoosters_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLootboxKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    LootboxKeyId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLootboxKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLootboxKeys_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryLootboxKeys_LootboxKeys_LootboxKeyId",
                        column: x => x.LootboxKeyId,
                        principalTable: "LootboxKeys",
                        principalColumn: "LootboxKeyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRoles_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveExpBoosters_UserId",
                table: "ActiveExpBoosters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardPacks_PacksId",
                table: "CardPacks",
                column: "PacksId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_SeriesId",
                table: "Cards",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CardStatistics_GuildId",
                table: "CardStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DNDStatistics_GuildId",
                table: "DNDStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Duets_DuoId",
                table: "Duets",
                column: "DuoId");

            migrationBuilder.CreateIndex(
                name: "IX_Duets_ProposerId",
                table: "Duets",
                column: "ProposerId");

            migrationBuilder.CreateIndex(
                name: "IX_DuetsStatistics_GuildId",
                table: "DuetsStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpBoosters_GuildId",
                table: "ExpBoosters",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_GuildId",
                table: "Inventories",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_UserId",
                table: "Inventories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExpBoosters_ExpBoosterId",
                table: "InventoryExpBoosters",
                column: "ExpBoosterId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExpBoosters_InventoryId",
                table: "InventoryExpBoosters",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLootboxKeys_InventoryId",
                table: "InventoryLootboxKeys",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLootboxKeys_LootboxKeyId",
                table: "InventoryLootboxKeys",
                column: "LootboxKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRoles_InventoryId",
                table: "InventoryRoles",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRoles_RoleId",
                table: "InventoryRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStatistics_GuildId",
                table: "InventoryStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_LootboxesStatistics_GuildId",
                table: "LootboxesStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_LootboxKeys_GuildId",
                table: "LootboxKeys",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_LootboxUserDatas_UserId",
                table: "LootboxUserDatas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersStatistics_GuildId",
                table: "MembersStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesStatistics_GuildId",
                table: "MessagesStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Packs_SeriesId",
                table: "Packs",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_GuildId",
                table: "Quests",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestsStatistics_GuildId",
                table: "QuestsStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId",
                table: "Roles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_GuildId",
                table: "Settings",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopRolesTries_UserId",
                table: "ShopRolesTries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildId",
                table: "Tickets",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketsStatistics_GuildId",
                table: "TicketsStatistics",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeoutAppaCatches_UserId",
                table: "TimeoutAppaCatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeoutCardCatches_UserId",
                table: "TimeoutCardCatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GuildId",
                table: "Transactions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PayerId",
                table: "Transactions",
                column: "PayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecipientId",
                table: "Transactions",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppas_AppaId",
                table: "UserAppas",
                column: "AppaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppas_UserId",
                table: "UserAppas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_GuildId",
                table: "UserCards",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_UserId",
                table: "UserCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPacks_GuildId",
                table: "UserPacks",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPacks_UserId",
                table: "UserPacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuests_UserId",
                table: "UserQuests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceStatistics_GuildId",
                table: "VoiceStatistics",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveExpBoosters");

            migrationBuilder.DropTable(
                name: "CardPacks");

            migrationBuilder.DropTable(
                name: "CardStatistics");

            migrationBuilder.DropTable(
                name: "ChannelsConfig");

            migrationBuilder.DropTable(
                name: "DNDStatistics");

            migrationBuilder.DropTable(
                name: "Duets");

            migrationBuilder.DropTable(
                name: "DuetsStatistics");

            migrationBuilder.DropTable(
                name: "EconomyConfigs");

            migrationBuilder.DropTable(
                name: "ExperienceConfig");

            migrationBuilder.DropTable(
                name: "InteractionsAssets");

            migrationBuilder.DropTable(
                name: "InventoryExpBoosters");

            migrationBuilder.DropTable(
                name: "InventoryLootboxKeys");

            migrationBuilder.DropTable(
                name: "InventoryRoles");

            migrationBuilder.DropTable(
                name: "InventoryStatistics");

            migrationBuilder.DropTable(
                name: "LogsConfig");

            migrationBuilder.DropTable(
                name: "LootboxesConfigs");

            migrationBuilder.DropTable(
                name: "LootboxesStatistics");

            migrationBuilder.DropTable(
                name: "LootboxRoles");

            migrationBuilder.DropTable(
                name: "LootboxUserDatas");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "MembersStatistics");

            migrationBuilder.DropTable(
                name: "MessagesStatistics");

            migrationBuilder.DropTable(
                name: "PacksConfigs");

            migrationBuilder.DropTable(
                name: "QuestsConfigs");

            migrationBuilder.DropTable(
                name: "QuestsStatistics");

            migrationBuilder.DropTable(
                name: "RolesConfig");

            migrationBuilder.DropTable(
                name: "ShopConfig");

            migrationBuilder.DropTable(
                name: "ShopKeys");

            migrationBuilder.DropTable(
                name: "ShopRoles");

            migrationBuilder.DropTable(
                name: "ShopRolesTries");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketsConfig");

            migrationBuilder.DropTable(
                name: "TicketsStatistics");

            migrationBuilder.DropTable(
                name: "TimeoutAppaCatches");

            migrationBuilder.DropTable(
                name: "TimeoutCardCatches");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserAppas");

            migrationBuilder.DropTable(
                name: "UserCards");

            migrationBuilder.DropTable(
                name: "UserPacks");

            migrationBuilder.DropTable(
                name: "UserQuests");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "VoiceConfig");

            migrationBuilder.DropTable(
                name: "VoiceStatistics");

            migrationBuilder.DropTable(
                name: "ExpBoosters");

            migrationBuilder.DropTable(
                name: "LootboxKeys");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Appas");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Packs");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
