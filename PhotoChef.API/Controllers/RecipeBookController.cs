using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Interfaces;
using PhotoChef.API.Dtos;
using System.Security.Claims;
using PhotoChef.API.Services;

namespace PhotoChef.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/recipebooks")]
    public class RecipeBookController : ControllerBase
    {
        private readonly IRecipeBookRepository _recipeBookRepository;
        private readonly ImageService _imageService;

        public RecipeBookController(IRecipeBookRepository recipeBookRepository, ImageService imageService)
        {
            _recipeBookRepository = recipeBookRepository;
            _imageService = imageService;
        }


        [HttpGet]
        public async Task<IActionResult> GetUserRecipeBooks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var recipeBooks = await _recipeBookRepository.GetRecipeBooksByUserIdAsync(userId);

            return Ok(recipeBooks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            // Fetch a single recipe book by ID
            var recipeBook = await _recipeBookRepository.GetBookByIdAsync(id);
            if (recipeBook == null)
                return NotFound(new { Message = "Recipe book not found." });

            return Ok(recipeBook);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBook([FromForm] RecipeBookDto recipeBookDto)
        {
            try
            {
                if (recipeBookDto == null || !ModelState.IsValid)
                    return BadRequest("Invalid recipe data.");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string imageUrl = null;
                if (recipeBookDto.imageFile != null)
                {
                    imageUrl = await _imageService.SaveImageAsync(recipeBookDto.imageFile, userId);
                }

                var recipeBook = new RecipeBook
                {
                    Title = recipeBookDto.Title,
                    Author = recipeBookDto.Author,
                    CoverImageUrl = imageUrl,
                    UserId = userId 
                };

                await _recipeBookRepository.AddBookAsync(recipeBook);

                return CreatedAtAction(nameof(GetBookById), new { id = recipeBook.Id }, recipeBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            // Delete a recipe book by ID
            await _recipeBookRepository.DeleteBookAsync(id);
            return NoContent();
        }
    }
}
