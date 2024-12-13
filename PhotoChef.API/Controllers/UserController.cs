using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhotoChef.API.Dtos;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using System.Security.Claims;
using System.Text;

namespace PhotoChef.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        public UserController(IUserRepository userRepository, IConfiguration configuration, TokenService tokenService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
        }


        // Register a new user
        [HttpPost("register")]
        [AllowAnonymous]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _userRepository.UserExistsAsync(dto.Username))
                    return BadRequest(new { Message = "Username already exists." });

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = dto.Password // Will be hashed in the repository
                };

                var registeredUser = await _userRepository.RegisterUserAsync(user);

                return Ok(new { Message = "User registered successfully.", User = registeredUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(dto.Username);
            var hasher = new PasswordHasher<User>();

            if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }
    }
}
