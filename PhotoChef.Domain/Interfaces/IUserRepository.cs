using PhotoChef.Domain.Entities;

namespace PhotoChef.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);
    }
}
