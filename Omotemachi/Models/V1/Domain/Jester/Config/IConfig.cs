using Omotemachi.Models.V1.Domain;

namespace Omotemachi.Models.V1.Domain.Jester.Config;

public interface IConfig
{
    public long GuildId { get; set; }
    public Guild? Guild { get; set; }
}
