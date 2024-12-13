using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PhotoChef.API.Dtos;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using PhotoChef.Domain.Enums;
using PhotoChef.Domain.Interfaces;
using PhotoChef.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace PhotoChef.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/recipes")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRecipeBookRepository _recipeBookRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ImageService _imageService;

        public RecipeController(IRecipeRepository recipeRepository, IWebHostEnvironment environment, ImageService imageService, IRecipeBookRepository recipeBookRepository)
        {
            _recipeRepository = recipeRepository;
            _environment = environment;
            _imageService = imageService;
            _recipeBookRepository = recipeBookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var recipes = await _recipeRepository.GetAllRecipesAsync(userId);
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var recipe = await _recipeRepository.GetRecipeByIdAsync(id,userId);
            if (recipe == null)
                return NotFound();
            return Ok(recipe);
        }
        
        [HttpPost]
        [SwaggerOperation(
    Summary = "Create a new recipe",
    Description = "This endpoint allows creating a new recipe by providing the details in the correct format. \n\n" +
                  "**Ingredients** should be provided as a JSON array of objects with the following structure:\n\n" +
                  "```json\n" +
                  "[{ \"name\": \"Flour\", \"quantity\": \"2 cups\" }, { \"name\": \"Sugar\", \"quantity\": \"1 cup\" }]\n" +
                  "```\n\n" +
                  "**Allergens** should be provided as a JSON array of integers representing their IDs:\n\n" +
                  "- 1: Gluten\n" +
                  "- 2: Nuts\n" +
                  "- 3: Dairy\n" +
                  "- 4: Eggs\n" +
                  "- 5: Soy\n" +
                  "- 6: Fish\n" +
                  "- 7: Shellfish\n" +
                  "- 8: Sesame\n" +
                  "- 9: Sulfites\n" +
                  "- 10: Mustard\n" +
                  "- 11: Celery\n" +
                  "- 12: Peanuts\n" +
                  "- 13: Lupin\n\n" +
                  "**Allergens example:**\n\n" +
                  "```json\n" +
                  "[1,3,5]\n" +
                  "```\n\n" +
                  "The `imageFile` field should be a valid image file (e.g., `.jpg`, `.jpeg`, or `.png`)."
)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddRecipe([FromForm] RecipeDto recipeDto)
        {
            if (recipeDto == null || !ModelState.IsValid)
                return BadRequest("Invalid recipe data.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Validate the RecipeDto
            var validationError = await ValidateRecipeDtoAsync(recipeDto, userId);
            if (validationError != null)
                return BadRequest(validationError);

            string imageUrl = null;

            if (recipeDto.imageFile != null)
            {
                imageUrl = await _imageService.SaveImageAsync(recipeDto.imageFile, userId);
            }

            // Create the recipe entity
            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                Instructions = recipeDto.Instructions,
                RecipeBookId = recipeDto.RecipeBookId,
                ImageUrl = imageUrl,
                Ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(recipeDto.Ingredients).Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList(),
                Allergens = JsonConvert.DeserializeObject<List<int>>(recipeDto.AllergenIds)
                .Select(id => (Allergen)id).ToList(),
                UserId = userId
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
        [SwaggerOperation(
    Summary = "Create a new recipe",
    Description = "This endpoint allows creating a new recipe by providing the details in the correct format. \n\n" +
                  "**Ingredients** should be provided as a JSON array of objects with the following structure:\n\n" +
                  "```json\n" +
                  "[{ \"name\": \"Flour\", \"quantity\": \"2 cups\" }, { \"name\": \"Sugar\", \"quantity\": \"1 cup\" }]\n" +
                  "```\n\n" +
                  "**Allergens** should be provided as a JSON array of integers representing their IDs:\n\n" +
                  "- 1: Gluten\n" +
                  "- 2: Nuts\n" +
                  "- 3: Dairy\n" +
                  "- 4: Eggs\n" +
                  "- 5: Soy\n" +
                  "- 6: Fish\n" +
                  "- 7: Shellfish\n" +
                  "- 8: Sesame\n" +
                  "- 9: Sulfites\n" +
                  "- 10: Mustard\n" +
                  "- 11: Celery\n" +
                  "- 12: Peanuts\n" +
                  "- 13: Lupin\n\n" +
                  "**Allergens example:**\n\n" +
                  "```json\n" +
                  "[1,3,5]\n" +
                  "```\n\n" +
                  "The `imageFile` field should be a valid image file (e.g., `.jpg`, `.jpeg`, or `.png`)."
)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromForm] RecipeDto recipeDto)
        {
            if (recipeDto == null || !ModelState.IsValid)
                return BadRequest("Invalid recipe data.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Validate the RecipeDto
            var validationError = await ValidateRecipeDtoAsync(recipeDto, userId);
            if (validationError != null)
                return BadRequest(validationError);

            // Additional logic to update the recipe
            var existingRecipe = await _recipeRepository.GetRecipeByIdAsync(id,userId);
            if (existingRecipe == null || existingRecipe.UserId != userId)
            {
                return NotFound("Recipe not found or does not belong to the user.");
            }

            existingRecipe.Name = recipeDto.Name;
            existingRecipe.Description = recipeDto.Description;
            existingRecipe.Instructions = recipeDto.Instructions;
            existingRecipe.RecipeBookId = recipeDto.RecipeBookId;
            existingRecipe.ImageUrl = recipeDto.imageFile != null
                ? await _imageService.SaveImageAsync(recipeDto.imageFile, userId)
                : existingRecipe.ImageUrl;
            existingRecipe.Ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(recipeDto.Ingredients)
                .Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList();
            existingRecipe.Allergens = JsonConvert.DeserializeObject<List<int>>(recipeDto.AllergenIds)
                .Select(id => (Allergen)id).ToList();

            await _recipeRepository.UpdateRecipeAsync(existingRecipe);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _recipeRepository.DeleteRecipeAsync(id, userId);
            return NoContent();
        }

        private async Task<string?> ValidateRecipeDtoAsync(RecipeDto recipeDto, int userId)
        {
            // Validate if the RecipeBook exists and belongs to the logged-in user
            var recipeBook = await _recipeBookRepository.GetBookByIdAsync(recipeDto.RecipeBookId);
            if (recipeBook == null || recipeBook.UserId != userId)
            {
                return "The specified RecipeBook does not exist or does not belong to the user.";
            }

            // Validate ingredients format
            if (!string.IsNullOrWhiteSpace(recipeDto.Ingredients))
            {
                try
                {
                    var ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(recipeDto.Ingredients);
                    if (ingredients.Any(i => string.IsNullOrWhiteSpace(i.Name) || string.IsNullOrWhiteSpace(i.Quantity)))
                    {
                        return "Each ingredient must have a valid name and quantity.";
                    }
                }
                catch
                {
                    return "Invalid ingredients format. Ingredients should be in JSON array format.";
                }
            }

            // Validate allergens format and check for invalid IDs
            try
            {
                var allergenIds = string.IsNullOrWhiteSpace(recipeDto.AllergenIds)
                    ? new List<int>()
                    : JsonConvert.DeserializeObject<List<int>>(recipeDto.AllergenIds);

                var invalidAllergenIds = allergenIds.Where(id => !Enum.IsDefined(typeof(Allergen), id)).ToList();
                if (invalidAllergenIds.Any())
                {
                    return $"Invalid allergen IDs: {string.Join(", ", invalidAllergenIds)}.";
                }
            }
            catch
            {
                return "Invalid allergens format. Allergens should be a JSON array of integers.";
            }

            // If all validations pass, return null
            return null;
        }

    }
}
