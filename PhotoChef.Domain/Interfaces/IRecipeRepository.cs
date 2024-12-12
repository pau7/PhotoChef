using PhotoChef.Domain.Entities;

namespace PhotoChef.Domain.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAllRecipesAsync(); 
        Task<Recipe?> GetRecipeByIdAsync(int id); 
        Task AddRecipeAsync(Recipe recipe); 
        Task UpdateRecipeAsync(Recipe recipe); 
        Task DeleteRecipeAsync(int id); 
    }
}
