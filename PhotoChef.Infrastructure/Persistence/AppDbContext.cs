using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Enums;

namespace PhotoChef.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RecipeBook> RecipeBooks { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User - RecipeBook (1:N)
            modelBuilder.Entity<RecipeBook>()
                .HasOne<User>()
                .WithMany(u => u.RecipeBooks)
                .HasForeignKey(rb => rb.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for user and recipe books

            // Configure RecipeBook - Recipe (1:N)
            modelBuilder.Entity<Recipe>()
                .HasOne<RecipeBook>()
                .WithMany(rb => rb.Recipes)
                .HasForeignKey(r => r.RecipeBookId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for recipe books and recipes

            // Configure Recipe - Ingredient (1:N)
            modelBuilder.Entity<Ingredient>()
                .HasOne<Recipe>()
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for recipes and ingredients

            // Configure conversion for Allergens in Recipe
            var allergensConverter = new ValueConverter<List<Allergen>, string>(
                v => string.Join(',', v), // Convert List<Allergen> to a comma-separated string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(a => Enum.Parse<Allergen>(a))
                      .ToList()
            );

            var allergensComparer = new ValueComparer<List<Allergen>>(
                (c1, c2) => c1.SequenceEqual(c2), // Compare two lists for equality
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Generate hash code for the list
                c => c.ToList() // Clone the list
            );

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Allergens)
                .HasConversion(allergensConverter) // Use the converter
                .Metadata.SetValueComparer(allergensComparer); // Set the comparer

            base.OnModelCreating(modelBuilder);
        }
    }
}
