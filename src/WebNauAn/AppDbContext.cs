using Microsoft.EntityFrameworkCore;
using WebNauAn.Models;

namespace WebNauAn
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SavedRecipe> SavedRecipes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserLike> UserLikes { get; set; }
    }
}