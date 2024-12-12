using PhotoChef.Domain.Enums;

namespace PhotoChef.Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string Description { get; set; }
        // Ingredients list
        public List<Ingredient> Ingredients { get; set; } = new();
        // Allergen list
        public List<Allergen> Allergens { get; set; } = new(); 
        public string Instructions { get; set; }
        public string ImageUrl { get; set; }

        public int RecipeBookId { get; set; }


    }
}
