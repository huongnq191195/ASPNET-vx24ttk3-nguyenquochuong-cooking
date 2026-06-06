namespace WebNauAn.Models
{
    public class SavedRecipe
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}