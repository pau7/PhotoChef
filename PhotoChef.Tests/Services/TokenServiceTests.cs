using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace PhotoChef.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("YourSecretKey12345");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("PhotoChefAPI");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("PhotoChefAPI");

            _tokenService = new TokenService(configurationMock.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturn_ValidJwtToken()
        {
            var user = new User { Id = 1, Username = "TestUser" };

            var token = _tokenService.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();
        }
    }
}
