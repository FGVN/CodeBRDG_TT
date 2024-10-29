using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeBRDG_TT.Models;

public class Dog
{
    [Key]
    [Column("name")]
    public string Name { get; set; }

    [Column("color")]
    public string Color { get; set; }

    [Column("tail_length")]
    public decimal TailLength { get; set; }

    [Column("weight")]
    public decimal Weight { get; set; }

    public Dog(string name, string color, decimal tailLength, decimal weight)
    {
        Name = name;
        Color = color;
        TailLength = tailLength;
        Weight = weight;
    }
}
