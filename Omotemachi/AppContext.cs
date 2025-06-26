using Microsoft.EntityFrameworkCore;
using Omotemachi.Models.V1;
using Newtonsoft.Json;
using Omotemachi.Models.V1.Jester.Items;
using Omotemachi.Models.V1.Jester.InventoryItems;
using Omotemachi.Models.V1.Jester.Interactions;
using Omotemachi.Models.V1.Jester.Config;
using Omotemachi.Models.V1.Jester.Lootboxes;
using Omotemachi.Models.V1.Jester.Quests;
using Omotemachi.Models.V1.Jester.Settings;
using Omotemachi.Models.V1.Jester.Shop;
using Omotemachi.Models.V1.Jester;
using Omotemachi.Models.V1.Logs;
using Omotemachi.Models.V1.Wacky.CCG;
using Omotemachi.Models.V1.Statistics;

namespace Omotemachi;

public class AppContext: DbContext
{
    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    public virtual DbSet<Guild> Guilds { get; set; }
    public virtual DbSet<User> Users { get; set; }

    // Members
    public virtual DbSet<Member> Members { get; set; }

    // Settings
    public virtual DbSet<Setting> Settings { get; set; }
    public virtual DbSet<UserSetting> UserSettings { get; set; }

    //Economy
    public virtual DbSet<Transaction> Transactions { get; set; }

    // Tickets
    public virtual DbSet<Ticket> Tickets { get; set; }

    // Configs
    public virtual DbSet<ShopConfig> ShopConfig { get; set; }
    public virtual DbSet<TicketsConfig> TicketsConfig { get; set; }
    public virtual DbSet<VoiceConfig> VoiceConfig { get; set; }
    public virtual DbSet<LogsConfig> LogsConfig { get; set; }
    public virtual DbSet<ExperienceConfig> ExperienceConfig { get; set; }
    public virtual DbSet<RolesConfig> RolesConfig { get; set; }
    public virtual DbSet<ChannelsConfig> ChannelsConfig { get; set; }
    public virtual DbSet<LootboxesConfig> LootboxesConfigs { get; set; }
    public virtual DbSet<EconomyConfig> EconomyConfigs { get; set; }
    public virtual DbSet<QuestsConfig> QuestsConfigs { get; set; }

    // Inventory
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<LootboxKey> LootboxKeys { get; set; }
    public virtual DbSet<ExpBooster> ExpBoosters { get; set; }
    public virtual DbSet<ActiveExpBooster> ActiveExpBoosters { get; set; }
    public virtual DbSet<InventoryExpBooster> InventoryExpBoosters { get; set; }
    public virtual DbSet<InventoryLootboxKey> InventoryLootboxKeys { get; set; }
    public virtual DbSet<InventoryRole> InventoryRoles { get; set; }
    public virtual DbSet<Inventory> Inventories { get; set; }

    // Lootboxes
    public virtual DbSet<LootboxRole> LootboxRoles { get; set; }
    public virtual DbSet<LootboxUserData> LootboxUserDatas { get; set; }

    // Shop
    public virtual DbSet<ShopRole> ShopRoles { get; set; }
    public virtual DbSet<ShopKey> ShopKeys { get; set; }
    public virtual DbSet<ShopRoleTries> ShopRolesTries { get; set; }

    // Duets
    public virtual DbSet<Duet> Duets { get; set; }

    // Interactions
    public virtual DbSet<InteractionsAsset> InteractionsAssets { get; set; }

    // Cards
    public virtual DbSet<Series> Series { get; set; }
    public virtual DbSet<Card> Cards { get; set; }
    public virtual DbSet<UserCard> UserCards { get; set; }
    public virtual DbSet<Pack> Packs { get; set; }
    public virtual DbSet<UserPack> UserPacks { get; set; }

    // Quests
    public virtual DbSet<Quest> Quests { get; set; }
    public virtual DbSet<UserQuest> UserQuests { get; set; }

    // Statistics
    public virtual DbSet<DNDStatistics> DNDStatistics { get; set; }
    public virtual DbSet<DuetsStatistics> DuetsStatistics { get; set; }
    public virtual DbSet<InventoryStatistics> InventoryStatistics { get; set; }
    public virtual DbSet<LootboxesStatistics> LootboxesStatistics { get; set; }
    public virtual DbSet<MembersStatistics> MembersStatistics { get; set; }
    public virtual DbSet<QuestsStatistics> QuestsStatistics { get; set; }
    public virtual DbSet<TicketsStatistics> TicketsStatistics { get; set; }
    public virtual DbSet<MessagesStatistics> MessagesStatistics { get; set; }
    public virtual DbSet<VoiceStatistics> VoiceStatistics { get; set; }
    public virtual DbSet<CardsStatistics> CardStatistics { get; set; }

    // Logs
    public virtual DbSet<JesterLog> JesterLogs { get; set; }
    public virtual DbSet<WackyLog> WackyLogs { get; set; }
    public virtual DbSet<WebLog> WebLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasKey(m => new { m.GuildId, m.UserId });
        modelBuilder.Entity<Duet>()
            .HasKey(d => new { d.GuildId, d.ProposerId, d.DuoId });
        modelBuilder.Entity<ActiveExpBooster>()
            .HasKey(aeb => new { aeb.GuildId, aeb.UserId });
        modelBuilder.Entity<LootboxRole>()
            .HasKey(lr => new { lr.GuildId, lr.GuildRoleId });
        modelBuilder.Entity<ShopRole>()
            .HasKey(lr => new { lr.GuildId, lr.GuildRoleId });
        modelBuilder.Entity<ShopKey>()
            .HasKey(lr => new { lr.GuildId, lr.LootboxType });
        modelBuilder.Entity<LootboxUserData>()
            .HasKey(lur => new { lur.GuildId, lur.UserId, lur.LootboxType });
        modelBuilder.Entity<ShopRoleTries>()
            .HasKey(srt => new { srt.GuildId, srt.UserId, srt.GuildRoleId });
        modelBuilder.Entity<UserQuest>()
            .HasKey(uq => new { uq.QuestId, uq.UserId });
        modelBuilder.Entity<UserSetting>()
            .HasKey(us => new { us.SettingId, us.UserId });
        modelBuilder.Entity<Card>()
            .HasMany(c => c.Packs)
            .WithMany(p => p.Cards)
            .UsingEntity(j => j.ToTable("CardPacks"));
        modelBuilder.Entity<UserCard>()
            .HasKey(uc => new { uc.CardId, uc.UserId, uc.GuildId });
        modelBuilder.Entity<UserPack>()
            .HasKey(uc => new { uc.PackId, uc.UserId, uc.GuildId });
        modelBuilder.Entity<LootboxUserData>()
            .Property(b => b.Data)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, int>>(v)!);
    }
}
