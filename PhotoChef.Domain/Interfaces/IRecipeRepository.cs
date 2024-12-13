using PhotoChef.Domain.Entities;

namespace PhotoChef.Domain.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAllRecipesAsync(int userId); 
        Task<Recipe?> GetRecipeByIdAsync(int id, int userId); 
        Task AddRecipeAsync(Recipe recipe); 
        Task UpdateRecipeAsync(Recipe recipe); 
        Task DeleteRecipeAsync(int id, int userId); 
    }
}
