using Microsoft.EntityFrameworkCore;
using WebNauAn;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebNauAnDb_V16;Trusted_Connection=True;MultipleActiveResultSets=true"));

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
            new WebNauAn.Models.Category { MucCha = "Món Chính", MucCon = "Món Việt" },
            new WebNauAn.Models.Category { MucCha = "Món Chính", MucCon = "Món Hàn" },
            new WebNauAn.Models.Category { MucCha = "Món Canh & Lẩu", MucCon = "Món Việt" },
            new WebNauAn.Models.Category { MucCha = "Món Canh & Lẩu", MucCon = "Món Hàn" },
            new WebNauAn.Models.Category { MucCha = "Món Ăn kèm", MucCon = "Salad" },
            new WebNauAn.Models.Category { MucCha = "Món Ăn kèm", MucCon = "Gỏi" },
            new WebNauAn.Models.Category { MucCha = "Món Ăn vặt", MucCon = "Món Việt" },
            new WebNauAn.Models.Category { MucCha = "Món Ăn vặt", MucCon = "Món Hàn" },
            new WebNauAn.Models.Category { MucCha = "Món Tráng miệng", MucCon = "Chè" },
            new WebNauAn.Models.Category { MucCha = "Món Tráng miệng", MucCon = "Bánh ngọt" }
        );
        context.SaveChanges();
    }
}

app.Run();