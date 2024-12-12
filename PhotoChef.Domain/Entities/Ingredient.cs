namespace PhotoChef.Domain.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        // Quantity of the ingredient (e.g., "200g", "1 cup")
        public string Quantity { get; set; }

        public int RecipeId { get; set; }
    }
}
