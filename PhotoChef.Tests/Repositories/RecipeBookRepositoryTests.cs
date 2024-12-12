using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoChef.Domain.Entities;
using PhotoChef.Infrastructure.Persistence;
using PhotoChef.Infrastructure.Repositories;
using Xunit;

namespace PhotoChef.Tests.Repositories
{
    public class RecipeBookRepositoryTests
    {
        private readonly RecipeBookRepository _recipeBookRepository;
        private readonly AppDbContext _context;

        public RecipeBookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _recipeBookRepository = new RecipeBookRepository(_context);
        }

        [Fact]
        public async Task AddBookAsync_ShouldAddRecipeBookToDatabase()
        {
            var recipeBook = new RecipeBook
            {
                Title = "My Recipe Book",
                Author = "Peter",
                UserId = 1,
                CoverImageUrl = "/images/test-cover.jpg"
            };

            await _recipeBookRepository.AddBookAsync(recipeBook);

            _context.RecipeBooks.Should().ContainSingle(rb => rb.Title == "My Recipe Book");
        }

        [Fact]
        public async Task GetRecipeBooksByUserIdAsync_ShouldReturnBooksForSpecificUser()
        {
            var recipeBooks = new List<RecipeBook>
            {
                new RecipeBook { Title = "Book 1", UserId = 1, Author="Peter", CoverImageUrl="test.png" },
                new RecipeBook { Title = "Book 2", UserId = 1, Author="Peter", CoverImageUrl="test.png" },
                new RecipeBook { Title = "Book 3", UserId = 2, Author="Peter", CoverImageUrl="test.png"}
            };
            await _context.RecipeBooks.AddRangeAsync(recipeBooks);
            await _context.SaveChangesAsync();

            var result = await _recipeBookRepository.GetRecipeBooksByUserIdAsync(1);

            result.Should().HaveCount(2);
            result.Select(rb => rb.Title).Should().Contain(new[] { "Book 1", "Book 2" });
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenIdExists()
        {
            var recipeBook = new RecipeBook { Title = "Book 1", UserId = 1, Author = "Peter", CoverImageUrl = "test.png" };
            await _context.RecipeBooks.AddAsync(recipeBook);
            await _context.SaveChangesAsync();

            var result = await _recipeBookRepository.GetBookByIdAsync(recipeBook.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Book 1");
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            var result = await _recipeBookRepository.GetBookByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldRemoveBookFromDatabase()
        {
            var recipeBook = new RecipeBook { Title = "Book to Delete", UserId = 1, Author = "Peter", CoverImageUrl = "test.png" };
            await _context.RecipeBooks.AddAsync(recipeBook);
            await _context.SaveChangesAsync();

            await _recipeBookRepository.DeleteBookAsync(recipeBook.Id);

            _context.RecipeBooks.Should().NotContain(rb => rb.Id == recipeBook.Id);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldDoNothing_WhenIdDoesNotExist()
        {
            await _recipeBookRepository.DeleteBookAsync(999);

            _context.RecipeBooks.Should().BeEmpty();
        }
    }
}
