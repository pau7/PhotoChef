using PhotoChef.Domain.Entities;

namespace PhotoChef.Domain.Interfaces
{
    public interface IRecipeBookRepository
    {
        Task<RecipeBook?> GetBookByIdAsync(int bookId);
        Task AddBookAsync(RecipeBook recipeBook);
        Task DeleteBookAsync(int bookId);
        Task<IEnumerable<RecipeBook>> GetRecipeBooksByUserIdAsync(int userId);
    }
}
