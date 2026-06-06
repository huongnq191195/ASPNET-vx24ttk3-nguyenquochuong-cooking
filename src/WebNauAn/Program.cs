using Microsoft.EntityFrameworkCore;
using WebNauAn;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebNauAnDb_V15;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Recipe}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new WebNauAn.Models.Category { MucCha = "Món chính", MucCon = "Món Việt" },
            new WebNauAn.Models.Category { MucCha = "Món chính", MucCon = "Món Hàn" },
            new WebNauAn.Models.Category { MucCha = "Món ăn vặt", MucCon = "Món Việt" },
            new WebNauAn.Models.Category { MucCha = "Món ăn vặt", MucCon = "Món Hàn" }
        );
        context.SaveChanges();
    }
}

app.Run();