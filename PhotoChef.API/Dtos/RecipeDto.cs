using PhotoChef.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhotoChef.API.Dtos
{
    public class RecipeDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Instructions { get; set; }
        [Required]
        public int RecipeBookId { get; set; }        
        public IFormFile imageFile { get; set; }
        [Required]
        public string Ingredients { get; set; }
        public string AllergenIds { get; set; }
    }
}
