namespace WebNauAn.Models
{
    public class UserLike
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RecipeId { get; set; }
    }
}