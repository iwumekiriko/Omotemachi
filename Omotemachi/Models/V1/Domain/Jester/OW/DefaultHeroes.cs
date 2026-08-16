using System.Diagnostics.Metrics;
using System.Security.Cryptography;

namespace Omotemachi.Models.V1.Domain.Jester.OW;

public class DefaultHeroes
{
    public static readonly string[] Tank =
    {
        "D.VA", "D.MON", "Domina", "Doomfist", "Hazard", "Junker Queen", "Mauga", "Orisa", "Ramattra", "REINHARDT", "Roadhog", "Sigma", "Winston", "Wrecking ball", "Zarya"
    };

    public static readonly string[] Damage =
    {
        "Anran", "Ashe", "Bastion", "Cassidy", "Echo", "Emre", "Freja", "Genji", "Hanzo", "Junkrat", "Mei", "Pharah", "Reaper", "Shion", "Sierra", "Sojourn", "SOLDIER: 76", "Sombra", "Symmetra", "TORBJORN LINDHOLM", "Tracer", "Vendetta", "Venture", "Widowmaker"
    };

    public static readonly string[] Support =
    {
        "Ana", "Baptiste", "Brigitte", "Illari", "JETPACK CAT", "Juno", "Kiriko", "Lifeweaver", "LUCIO", "Mercy", "Mizuki", "Moira", "Wuyang", "Zenyatta"
    };
}
