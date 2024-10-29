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
            new Dog ("Buddy", "brown", 30, 6 ),
            new Dog ("Rex", "white & black", 20, 5 ),
            new Dog ("Max", "ultramarine", 25, 4 ),
            new Dog ("Charlie", "golden", 28, 7 ),
            new Dog ("Daisy", "black", 22, 3 )
        };

        SeedDogs(context, dogs); 
    }
}
