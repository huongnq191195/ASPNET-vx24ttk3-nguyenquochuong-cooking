using System;

namespace WebNauAn.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayDang { get; set; } = DateTime.Now;

        public Recipe Recipe { get; set; } = null!;
    }
}