using CodeBRDG_TT.Data;
using CodeBRDG_TT.Models;
using System.Collections.Generic;
using System.Linq;

public static class DatabaseSeeder
{
    public static void SeedDogs(AppDbContext context, IList<Dog> dogs)
    {
        if (!context.Dogs.Any()) // Check if the Dogs table is empty
        {
            context.Dogs.AddRange(dogs);
            context.SaveChanges();
        }
    }

    public static void SeedDefaultDogs(AppDbContext context)
    {
        var dogs = new List<Dog>
        {
            new Dog { name = "Buddy", weight = 30, tail_length = 6, color = "brown" },
            new Dog { name = "Rex", weight = 20, tail_length = 5, color = "white & black" },
            new Dog { name = "Max", weight = 25, tail_length = 4, color = "ultramarine" },
            new Dog { name = "Charlie", weight = 28, tail_length = 7, color = "golden" },
            new Dog { name = "Daisy", weight = 22, tail_length = 3, color = "black" }
        };

        SeedDogs(context, dogs); 
    }
}
