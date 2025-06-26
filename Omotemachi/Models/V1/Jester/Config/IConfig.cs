namespace Omotemachi.Models.V1.Jester.Config;

public interface IConfig
{
    public long GuildId { get; set; }
    public Guild? Guild { get; set; }
}
