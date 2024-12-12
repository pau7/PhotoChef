using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using PhotoChef.Infrastructure.Persistence;

namespace PhotoChef.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new Exception("Username already exists.");

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }


        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return verificationResult == PasswordVerificationResult.Success;
        }
        public async Task<List<RecipeBook>> GetRecipeBooksForUserAsync(int userId)
        {            
            return await _context.RecipeBooks
                .Where(rb => rb.Id % userId == 0)
                .ToListAsync();
        }
    }
}
