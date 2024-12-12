using Microsoft.EntityFrameworkCore;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using PhotoChef.Infrastructure.Persistence;

namespace PhotoChef.Infrastructure.Repositories
{
    public class RecipeBookRepository : IRecipeBookRepository
    {
        private readonly AppDbContext _context;

        public RecipeBookRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RecipeBook>> GetRecipeBooksByUserIdAsync(int userId)
        {
            return await _context.RecipeBooks
                .Where(rb => rb.UserId == userId)
                .ToListAsync();
        }

        public async Task<RecipeBook?> GetBookByIdAsync(int bookId)
        {
            return await _context.RecipeBooks
                .FirstOrDefaultAsync(rb => rb.Id == bookId);
        }

        public async Task AddBookAsync(RecipeBook recipeBook)
        {
            _context.RecipeBooks.Add(recipeBook);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int bookId)
        {
            var recipeBook = await GetBookByIdAsync(bookId);
            if (recipeBook != null)
            {
                _context.RecipeBooks.Remove(recipeBook);
                await _context.SaveChangesAsync();
            }
        }
    }
}
