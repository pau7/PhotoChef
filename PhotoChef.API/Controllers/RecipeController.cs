using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PhotoChef.API.Dtos;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Enums;
using PhotoChef.Domain.Interfaces;
using System.Security.Claims;

namespace PhotoChef.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/recipes")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ImageService _imageService;

        public RecipeController(IRecipeRepository recipeRepository, IWebHostEnvironment environment, ImageService imageService)
        {
            _recipeRepository = recipeRepository;
            _environment = environment;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var recipes = await _recipeRepository.GetAllRecipesAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddRecipe([FromForm] RecipeDto recipeDto)
        {

            if (recipeDto == null || !ModelState.IsValid)
                return BadRequest("Invalid recipe data.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");            
            string imageUrl = null;
            if (recipeDto.imageFile != null)
            {
                imageUrl = await _imageService.SaveImageAsync(recipeDto.imageFile, userId);
            }
            List<IngredientDto> ingredients = new List<IngredientDto>();
            if (!string.IsNullOrWhiteSpace(recipeDto.Ingredients))
            {
                try
                {
                    ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(recipeDto.Ingredients);
                }
                catch
                {
                    return BadRequest("Invalid ingredients format.");
                }
            }
            var allergenIds = string.IsNullOrWhiteSpace(recipeDto.AllergenIds)
            ? new List<int>()
            : JsonConvert.DeserializeObject<List<int>>(recipeDto.AllergenIds);
            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                Instructions = recipeDto.Instructions,
                RecipeBookId = recipeDto.RecipeBookId,
                ImageUrl = imageUrl,
                Ingredients = ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList(),
                Allergens = allergenIds.Select(id => (Allergen)id).ToList()
            };

            await _recipeRepository.AddRecipeAsync(recipe);

            return CreatedAtAction(
                nameof(RecipeController.GetRecipeById),
                "Recipe",
                new { id = recipe.Id },
                recipe
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
                return BadRequest("Recipe ID mismatch.");

            await _recipeRepository.UpdateRecipeAsync(recipe);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            await _recipeRepository.DeleteRecipeAsync(id);
            return NoContent();
        }
    }
}
