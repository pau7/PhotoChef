using System.ComponentModel.DataAnnotations;

namespace PhotoChef.API.Dtos
{
    public class IngredientDto        
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Quantity { get; set; } // Ejemplo: "200g", "1 cup"
    }
}
