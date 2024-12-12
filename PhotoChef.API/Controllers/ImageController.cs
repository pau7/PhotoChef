    using global::PhotoChef.API.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PhotoChef.Domain.Interfaces;
    using System.Security.Claims;

    namespace PhotoChef.API.Controllers
    {
        [ApiController]
        [Authorize]
        [Route("api/images")]
        public class ImageController : ControllerBase
        {
            private readonly ImageService _imageService;

            public ImageController(ImageService imageService)
            {
                _imageService = imageService;
            }

            [HttpPost("upload")]
            [Consumes("multipart/form-data")]
            public async Task<IActionResult> UploadImage(IFormFile file)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                try
                {
                    var imageUrl = await _imageService.SaveImageAsync(file, userId);
                    return Ok(new { Url = imageUrl });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while uploading the image.", Error = ex.Message });
                }
            }

            [HttpGet]
            public IActionResult GetUserImages()
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId.ToString());
                if (!Directory.Exists(userFolder))
                    return Ok(new List<string>());

                var images = Directory.GetFiles(userFolder)
                    .Select(file => $"/images/{userId}/{Path.GetFileName(file)}")
                    .ToList();

                return Ok(images);
            }

            [HttpDelete("{fileName}")]
            public IActionResult DeleteUserImage(string fileName)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                try
                {
                    var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId.ToString());
                    var filePath = Path.Combine(userFolder, fileName);

                    if (!System.IO.File.Exists(filePath))
                        return NotFound(new { Message = "Image not found." });

                    System.IO.File.Delete(filePath);
                    return Ok(new { Message = "Image deleted successfully.", FileName = fileName });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while deleting the image.", Error = ex.Message });
                }
            }

            [HttpDelete("cleanup")]
            public async Task<IActionResult> CleanupUnusedImages([FromServices] IRecipeRepository recipeRepository)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId.ToString());

                if (!Directory.Exists(userFolder))
                    return Ok("No images folder found.");

                var allImages = Directory.GetFiles(userFolder)
                    .Select(file => Path.GetFileName(file))
                    .ToList();

                var associatedImages = (await recipeRepository.GetAllRecipesAsync())
                    .Where(r => !string.IsNullOrEmpty(r.ImageUrl) && r.ImageUrl.Contains($"/images/{userId}/"))
                    .Select(r => Path.GetFileName(r.ImageUrl))
                    .ToList();

                var orphanImages = allImages.Except(associatedImages).ToList();

                foreach (var imageName in orphanImages)
                {
                    var filePath = Path.Combine(userFolder, imageName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                return Ok(new { Message = $"{orphanImages.Count} unused images deleted.", DeletedImages = orphanImages });
            }
        }
    }


