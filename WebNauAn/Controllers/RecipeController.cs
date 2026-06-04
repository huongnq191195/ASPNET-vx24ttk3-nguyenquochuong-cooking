using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebNauAn.Models;

namespace WebNauAn.Controllers
{
    public class RecipeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost; // Thêm cái này để quản lý thư mục lưu ảnh

        public RecipeController(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // 1. TRANG CHỦ: Chỉ hiển thị món ăn ĐÃ ĐƯỢC DUYỆT
        public IActionResult Index()
        {
            var dsMonAn = _context.Recipes.Include(r => r.Category).Where(r => r.IsApproved == true).ToList();
            return View(dsMonAn);
        }

        // 2. TRANG ĐĂNG BÀI 
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Recipe recipe, IFormFile AnhFile) // Nhận file ảnh từ giao diện
        {
            // Xử lý tải ảnh và lưu vào thư mục wwwroot/images
            if (AnhFile != null && AnhFile.Length > 0)
            {
                string folderUpload = Path.Combine(_webHost.WebRootPath, "images");

                // Nếu thư mục images chưa tồn tại thì tự tạo mới
                if (!Directory.Exists(folderUpload))
                {
                    Directory.CreateDirectory(folderUpload);
                }

                // Tạo tên ảnh ngẫu nhiên để không bị trùng file
                string tenFileDocNhat = Guid.NewGuid().ToString() + "_" + AnhFile.FileName;
                string duongDanFile = Path.Combine(folderUpload, tenFileDocNhat);

                // Copy file ảnh vào thư mục của web
                using (var stream = new FileStream(duongDanFile, FileMode.Create))
                {
                    AnhFile.CopyTo(stream);
                }

                // Lưu đường dẫn ảnh vào Database để hiển thị
                recipe.HinhanhUrl = "/images/" + tenFileDocNhat;
            }

            recipe.IsApproved = false; // Luôn ép buộc chờ duyệt
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 3. TRANG ADMIN: Xem toàn bộ bài để Duyệt hoặc Xóa
        public IActionResult AdminDuyet()
        {
            // KHÓA TRANG: Nếu không phải Admin thì đá ra trang Đăng nhập liền
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") { return RedirectToAction("Login", "Account"); }

            var dsChoDuyet = _context.Recipes.Include(r => r.Category).ToList();
            return View(dsChoDuyet);
        }

        // Bấm nút Duyệt bài
        public IActionResult Approve(int id)
        {
            var mon = _context.Recipes.Find(id);
            if (mon != null)
            {
                mon.IsApproved = true;
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDuyet");
        }
        // Tính năng Admin bấm nút Xóa bài
        public IActionResult Delete(int id)
        {
            var mon = _context.Recipes.Find(id);
            if (mon != null)
            {
                // 1. Xóa file ảnh trong thư mục wwwroot/images trước cho nhẹ máy (nếu có)
                if (!string.IsNullOrEmpty(mon.HinhanhUrl))
                {
                    var duongDanAnh = Path.Combine(_webHost.WebRootPath, mon.HinhanhUrl.TrimStart('/'));
                    if (System.IO.File.Exists(duongDanAnh))
                    {
                        System.IO.File.Delete(duongDanAnh);
                    }
                }

                // 2. Xóa thông tin món ăn trong Database
                _context.Recipes.Remove(mon);
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDuyet");
        }
    }
}