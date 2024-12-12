namespace PhotoChef.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public List<RecipeBook> RecipeBooks { get; set; } = new();
    }
}
