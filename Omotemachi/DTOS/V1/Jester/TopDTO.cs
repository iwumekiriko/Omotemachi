namespace Omotemachi.DTOS.V1.Jester;

public class TopDTO
{
    public List<UserRank> Top { get; set; } = [];
    public UserRank? RequestedUser { get; set; }
}

public class UserRank
{
    public int Rank { get; set; }
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public object Stats { get; set; }
}