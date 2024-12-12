namespace PhotoChef.Domain.Entities
{
    public class RecipeBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public List<Recipe> Recipes { get; set; } = new();
        public int UserId { get; set; }

    }
}
