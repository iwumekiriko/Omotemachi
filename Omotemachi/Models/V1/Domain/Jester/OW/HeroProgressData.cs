namespace Omotemachi.Models.V1.Domain.Jester.OW;

public class HeroProgressData
{
    public Dictionary<string, int> Tank { get; set; } = new();
    public Dictionary<string, int> Damage { get; set; } = new();
    public Dictionary<string, int> Support { get; set; } = new();

    public void EnsureAllHeroes()
    {
        Ensure(Tank, DefaultHeroes.Tank);
        Ensure(Damage, DefaultHeroes.Damage);
        Ensure(Support, DefaultHeroes.Support);
    }
    public void Reset()
    {
        EnsureAllHeroes();

        foreach (var key in Tank.Keys.ToList()) Tank[key] = 0;
        foreach (var key in Damage.Keys.ToList()) Damage[key] = 0;
        foreach (var key in Support.Keys.ToList()) Support[key] = 0;
    }
    private static void Ensure(Dictionary<string, int> dict, string[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (!dict.ContainsKey(hero))
                dict[hero] = 0;
        }
    }

}
