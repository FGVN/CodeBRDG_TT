using Microsoft.EntityFrameworkCore;
using CodeBRDG_TT.Models;

namespace CodeBRDG_TT.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Dog> Dogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dog>(entity =>
        {
            entity.Property(d => d.TailLength)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            entity.Property(d => d.Weight)
                .HasColumnType("decimal(18, 2)") 
                .IsRequired(); 
        });

        base.OnModelCreating(modelBuilder);
    }
}
