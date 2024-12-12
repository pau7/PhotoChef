using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoChef.API.Services;
using PhotoChef.Domain.Interfaces;
using System.Security.Claims;

namespace PhotoChef.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly PdfGeneratorService _pdfGeneratorService;
        private readonly IRecipeBookRepository _recipeBookRepository;
        private readonly IRecipeRepository _recipeRepository;

        public PdfController(PdfGeneratorService pdfGeneratorService, IRecipeBookRepository recipeBookRepository, IRecipeRepository recipeRepository)
        {
            _pdfGeneratorService = pdfGeneratorService;
            _recipeBookRepository = recipeBookRepository;
            _recipeRepository = recipeRepository;
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetRecipeBook(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var recipeBook = await _recipeBookRepository.GetBookByIdAsync(bookId);
            if (recipeBook == null || recipeBook.UserId != userId)
            {
                return Unauthorized("You do not have access to this recipe book.");
            }

            var recipes = (await _recipeRepository.GetAllRecipesAsync())
                .Where(r => r.RecipeBookId == bookId)
                .ToList();

            if (!recipes.Any())
            {
                return NotFound("No recipes found in this recipe book.");
            }

            var pdfBytes = _pdfGeneratorService.GenerateRecipeBook(recipeBook.Title, recipeBook.Author, recipeBook.CoverImageUrl, recipes, userId);

            return File(pdfBytes, "application/pdf", $"{recipeBook.Title}.pdf");
        }
    }
}
