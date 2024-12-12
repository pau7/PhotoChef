using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PhotoChef.API.Controllers;
using PhotoChef.API.Dtos;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PhotoChef.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly TokenService _tokenService;

        public UserControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("YourSecretKey12345");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("PhotoChefAPI");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("PhotoChefAPI");

            _tokenService = new TokenService(configurationMock.Object); // Usa una instancia real del servicio

            _controller = new UserController(_mockRepo.Object, configurationMock.Object, _tokenService);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsSuccessfullyRegistered()
        {
            var dto = new RegisterDto { Username = "newUser", Password = "Password123" };

            _mockRepo.Setup(repo => repo.UserExistsAsync(dto.Username)).ReturnsAsync(false);
            _mockRepo.Setup(repo => repo.RegisterUserAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Username = dto.Username });

            var result = await _controller.Register(dto);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var message = okResult.Value.ToString();
            message.Should().Contain("User registered successfully.");
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
        {
            var dto = new RegisterDto { Username = "existingUser", Password = "Password123" };

            _mockRepo.Setup(repo => repo.UserExistsAsync(dto.Username)).ReturnsAsync(true);

            var result = await _controller.Register(dto);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var message = badRequestResult.Value.ToString();
            message.Should().Contain("Username already exists.");
        }

        [Fact]
        public async Task Register_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            var dto = new RegisterDto { Username = "errorUser", Password = "Password123" };

            _mockRepo.Setup(repo => repo.UserExistsAsync(dto.Username)).ThrowsAsync(new System.Exception("Database error"));

            var result = await _controller.Register(dto);

            var errorResult = result as ObjectResult;
            errorResult.Should().NotBeNull();
            errorResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var message = errorResult.Value.ToString();
            message.Should().Contain("Database error");
        }

        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var dto = new LoginDto { Username = "validUser", Password = "Password123" };

            var user = new User { Id = 1, Username = dto.Username, PasswordHash = "hashedPassword" };

            _mockRepo.Setup(repo => repo.GetUserByUsernameAsync(dto.Username)).ReturnsAsync(user);

            var hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(user, dto.Password);
            user.PasswordHash = hashedPassword;

            var result = await _controller.Login(dto);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = okResult.Value as dynamic;

            var token = response?.Token;
            token.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var dto = new LoginDto { Username = "invalidUser", Password = "wrongPassword" };

            _mockRepo.Setup(repo => repo.GetUserByUsernameAsync(dto.Username)).ReturnsAsync((User)null);

            var result = await _controller.Login(dto);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

            var response = unauthorizedResult.Value.ToString();
            response.Should().NotBeNull();
            response.Should().Contain("Invalid username or password.");
        }


        [Fact]
        public async Task GetRecipeBooks_ShouldReturnOk_WhenBooksExist()
        {
            var userId = 1;
            var books = new List<RecipeBook>
            {
                new RecipeBook { Id = 1, Title = "Book 1", UserId = userId },
                new RecipeBook { Id = 2, Title = "Book 2", UserId = userId }
            };

            _mockRepo.Setup(repo => repo.GetRecipeBooksForUserAsync(userId)).ReturnsAsync(books);

            var result = await _controller.GetRecipeBooks(userId);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = okResult.Value as List<RecipeBook>;
            response.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetRecipeBooks_ShouldReturnNotFound_WhenNoBooksExist()
        {
            var userId = 1;

            _mockRepo.Setup(repo => repo.GetRecipeBooksForUserAsync(userId)).ReturnsAsync(new List<RecipeBook>());

            var result = await _controller.GetRecipeBooks(userId);

            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var response = notFoundResult.Value.ToString();
            response.Should().NotBeNull();
            response.Should().Contain("No recipe books found for this user.");
        }
    }
    
}
