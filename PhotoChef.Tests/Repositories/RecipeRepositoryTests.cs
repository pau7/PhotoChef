using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Enums;
using PhotoChef.Infrastructure.Persistence;
using PhotoChef.Infrastructure.Repositories;
using Xunit;

namespace PhotoChef.Tests.Repositories
{
    public class RecipeRepositoryTests
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly AppDbContext _context;

        public RecipeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _recipeRepository = new RecipeRepository(_context);
        }

        [Fact]
        public async Task AddRecipeAsync_ShouldAddRecipeToDatabase()
        {
            var recipe = new Recipe
            {
                Name = "Test Recipe",
                Description = "Delicious test recipe",
                Instructions = "Test instructions",
                Allergens = new List<Allergen> { Allergen.Gluten },
                ImageUrl = "/images/test.jpg",
                Ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "Flour", Quantity = "2 cups" },
                    new Ingredient { Name = "Sugar", Quantity = "1 cup" }
                }
            };

            await _recipeRepository.AddRecipeAsync(recipe);

            _context.Recipes.Should().ContainSingle(r => r.Name == "Test Recipe");
        }

        [Fact]
        public async Task GetAllRecipesAsync_ShouldReturnAllRecipes()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { Name = "Recipe 1", Description = "Description 1", Instructions="Instructions 1", ImageUrl="test.png" },
                new Recipe { Name = "Recipe 2", Description = "Description 2", Instructions="Instructions 2", ImageUrl="test.png" }
            };
            await _context.Recipes.AddRangeAsync(recipes);
            await _context.SaveChangesAsync();

            var result = await _recipeRepository.GetAllRecipesAsync();

            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(new[] { "Recipe 1", "Recipe 2" });
        }

        [Fact]
        public async Task GetRecipeByIdAsync_ShouldReturnRecipe_WhenIdExists()
        {
            var recipe = new Recipe { Name = "Recipe 1", Description = "Description 1", Instructions="Instructions 1", ImageUrl="test.png" };
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            var result = await _recipeRepository.GetRecipeByIdAsync(recipe.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Recipe 1");
        }

        [Fact]
        public async Task GetRecipeByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            var result = await _recipeRepository.GetRecipeByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateRecipeAsync_ShouldUpdateExistingRecipe()
        {
            var recipe = new Recipe { Name = "Recipe 1", Description = "Description 1", Instructions = "Instructions 1", ImageUrl = "test.png" };
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            recipe.Name = "Updated Recipe";
            recipe.Description = "Updated Description";

            await _recipeRepository.UpdateRecipeAsync(recipe);

            var updatedRecipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == recipe.Id);
            updatedRecipe.Should().NotBeNull();
            updatedRecipe!.Name.Should().Be("Updated Recipe");
            updatedRecipe.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task DeleteRecipeAsync_ShouldRemoveRecipeFromDatabase()
        {
            var recipe = new Recipe { Name = "Recipe to Delete", Description = "Description", Instructions = "Instructions 1", ImageUrl = "test.png" };
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            await _recipeRepository.DeleteRecipeAsync(recipe.Id);

            _context.Recipes.Should().NotContain(r => r.Id == recipe.Id);
        }
    }
}
