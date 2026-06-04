using Microsoft.EntityFrameworkCore;
using WebNauAn;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(); // Kích hoạt bộ nhớ đăng nhập
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebNauAnDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession(); // Cho phép web sử dụng bộ nhớ đăng nhập

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Đoạn code tự động tạo danh mục mẫu đã bỏ Id để SQL tự tăng
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new WebNauAn.Models.Category { TenQuocGia = "Món Việt", LoaiMon = "Món chính" },
            new WebNauAn.Models.Category { TenQuocGia = "Món Hàn", LoaiMon = "Món chính" },
            new WebNauAn.Models.Category { TenQuocGia = "Món Thái", LoaiMon = "Món khai vị" }
        );
        context.SaveChanges();
    }
}

app.Run();
