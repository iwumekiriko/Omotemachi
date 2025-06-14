using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DellArteAPI.Models.V1.Jester.Items;

public class Coin : Item
{
    public int Amount { get; set; }
}
