using PhotoChef.Domain.Entities;

namespace PhotoChef.API.Dtos
{
        public class RecipeBookDto
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public IFormFile imageFile { get; set; }
    }
}
