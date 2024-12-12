using FluentAssertions;
using Moq;
using PhotoChef.API.Services;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PhotoChef.Tests.Services
{
    public class ImageServiceTests
    {
        private readonly ImageService _imageService;

        public ImageServiceTests()
        {
            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(env => env.WebRootPath).Returns("wwwroot");
            _imageService = new ImageService(environmentMock.Object);
        }

        [Fact]
        public void SaveImage_ShouldThrowException_WhenInvalidFileProvided()
        {
            var exception = Record.ExceptionAsync(() => _imageService.SaveImageAsync(null, 1));

            exception.Should().NotBeNull();
            exception.Result.Should().BeOfType<ArgumentException>();
        }
        [Fact]
        public async Task SaveImage_ShouldReturnValidUrl_WhenValidFileProvided()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("valid-image.png");
            fileMock.Setup(f => f.Length).Returns(1024); // 1 KB
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            var result = await _imageService.SaveImageAsync(fileMock.Object, 1);

            result.Should().Contain("/images/1/");
        }
        [Fact]
        public async Task SaveImage_ShouldReturnFalse_WhenImageIsBiggerThanMax()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("valid-image.png");
            fileMock.Setup(f => f.Length).Returns(1024*1024*6); // 6 MB
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");


            var exception = Record.ExceptionAsync(() => _imageService.SaveImageAsync(fileMock.Object, 1));

            exception.Should().NotBeNull();
            exception.Result.Should().BeOfType<InvalidOperationException>();
        }
        [Fact]
        public async Task SaveImage_ShouldReturnFalse_WhenContentTypeIsWrong()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("valid-image.png");
            fileMock.Setup(f => f.Length).Returns(1024); // 1 KB
            fileMock.Setup(f => f.ContentType).Returns("ge/jp");

            var exception = Record.ExceptionAsync(() => _imageService.SaveImageAsync(fileMock.Object, 1));

            exception.Should().NotBeNull();
            exception.Result.Should().BeOfType<InvalidOperationException>();
        }
        [Fact]
        public async Task SaveImage_ShouldReturnFalse_WhenWrongExtensionProvided()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("valid-image.pg");
            fileMock.Setup(f => f.Length).Returns(1024); // 1 KB
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            var exception = Record.ExceptionAsync(() => _imageService.SaveImageAsync(fileMock.Object, 1));

            exception.Should().NotBeNull();
            exception.Result.Should().BeOfType<InvalidOperationException>();
        }
        [Fact]
        public void DeleteImage_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            var invalidFileName = "non-existing-image.png";

            var exception = Assert.Throws<FileNotFoundException>(() =>
            {
                _imageService.DeleteImage(invalidFileName);
            });

            exception.Message.Should().Be("Image not found.");
        }


    }
}
