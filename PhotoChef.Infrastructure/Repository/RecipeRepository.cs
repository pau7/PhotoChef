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

        public async Task<List<Recipe>> GetAllRecipesAsync(int userId)
        {

            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id, int userId)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where (r => r.UserId == userId)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(int id, int userId)
        {
            var recipe = await _context.Recipes.Where(x=>x.UserId == userId).FirstOrDefaultAsync(r => r.Id == id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}
