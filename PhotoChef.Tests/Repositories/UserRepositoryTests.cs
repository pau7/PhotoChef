using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PhotoChef.Domain.Entities;
using PhotoChef.Infrastructure.Persistence;
using PhotoChef.Infrastructure.Repositories;
using Xunit;

namespace PhotoChef.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenUsernameIsUnique()
        {            
            var user = new User { Username = "TestUser", PasswordHash = "TestPassword" };
            
            var result = await _userRepository.RegisterUserAsync(user);
           
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Username.Should().Be("TestUser");
            _context.Users.Count().Should().Be(1);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameExists()
        {            
            var user1 = new User { Username = "TestUser", PasswordHash = "TestPassword1" };
            var user2 = new User { Username = "TestUser", PasswordHash = "TestPassword2" };
            await _userRepository.RegisterUserAsync(user1);
            
            Func<Task> act = async () => await _userRepository.RegisterUserAsync(user2);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Username already exists.");
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
        {            
            var user = new User { Username = "TestUser", PasswordHash = "TestPassword" };
            await _userRepository.RegisterUserAsync(user);
            
            var result = await _userRepository.GetUserByUsernameAsync("TestUser");
            
            result.Should().NotBeNull();
            result.Username.Should().Be("TestUser");
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUsernameDoesNotExist()
        {            
            var result = await _userRepository.GetUserByUsernameAsync("NonExistentUser");
            
            result.Should().BeNull();
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnTrue_WhenUserExists()
        {            
            var user = new User { Username = "TestUser", PasswordHash = "TestPassword" };
            await _userRepository.RegisterUserAsync(user);
            
            var result = await _userRepository.UserExistsAsync("TestUser");
            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {            
            var result = await _userRepository.UserExistsAsync("NonExistentUser");
            
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnTrue_WhenCredentialsAreValid()
        {            
            var user = new User { Username = "TestUser", PasswordHash = "TestPassword" };
            await _userRepository.RegisterUserAsync(user);
            
            var result = await _userRepository.ValidateUserAsync("TestUser", "TestPassword");
            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnFalse_WhenPasswordIsInvalid()
        {            
            var user = new User { Username = "TestUser", PasswordHash = "TestPassword" };
            await _userRepository.RegisterUserAsync(user);
            
            var result = await _userRepository.ValidateUserAsync("TestUser", "WrongPassword");
            
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
        {            
            var result = await _userRepository.ValidateUserAsync("NonExistentUser", "TestPassword");
            
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetRecipeBooksForUserAsync_ShouldReturnBooks_ForGivenUserId()
        {            
            var books = new List<RecipeBook>
            {
                new RecipeBook { Id = 2, Title = "Book 1", Author="Peter", CoverImageUrl="cover.png" },
                new RecipeBook { Id = 4, Title = "Book 2", Author="Peter", CoverImageUrl="cover.png" }
            };
            _context.RecipeBooks.AddRange(books);
            await _context.SaveChangesAsync();
            
            var result = await _userRepository.GetRecipeBooksForUserAsync(2);
            
            result.Should().HaveCount(2);
            result.Select(b => b.Title).Should().Contain(new[] { "Book 1", "Book 2" });
        }
    }
}
