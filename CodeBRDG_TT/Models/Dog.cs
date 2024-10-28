using System.ComponentModel.DataAnnotations;

namespace CodeBRDG_TT.Models;

public class Dog
{
    [Key]
    public string name { get; set; }
    public string color { get; set; }
    public decimal tail_length { get; set; }
    public decimal weight { get; set; }
}
