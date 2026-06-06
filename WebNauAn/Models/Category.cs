using System.Collections.Generic;

namespace WebNauAn.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string MucCha { get; set; } = string.Empty;
        public string MucCon { get; set; } = string.Empty;

        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}