using Microsoft.EntityFrameworkCore;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using PhotoChef.Infrastructure.Persistence;

namespace PhotoChef.Infrastructure.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            var existingRecipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existingRecipe != null)
            {
                existingRecipe.Name = recipe.Name;
                existingRecipe.Description = recipe.Description;
                existingRecipe.Ingredients = recipe.Ingredients;
                existingRecipe.Instructions = recipe.Instructions;
                existingRecipe.Allergens = recipe.Allergens;
                existingRecipe.ImageUrl = recipe.ImageUrl;

                _context.Recipes.Update(existingRecipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}
