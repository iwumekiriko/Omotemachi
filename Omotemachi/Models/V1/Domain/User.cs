using System.ComponentModel.DataAnnotations;

namespace Omotemachi.Models.V1;

public class User
{
    [Key]
    public long Id { get; set; }
}