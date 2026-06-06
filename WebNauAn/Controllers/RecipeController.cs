using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using WebNauAn.Models;
using System.Collections.Generic;

namespace WebNauAn.Controllers
{
    public class RecipeController : Controller
    {
        private readonly AppDbContext _context;

        public RecipeController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. CÁC TÍNH NĂNG XEM VÀ HIỂN THỊ
        // ==========================================

        public IActionResult Index(string searchString, string ingredientsString, int? categoryId)
        {
            var currentUsername = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            ViewBag.SearchString = searchString;
            ViewBag.IngredientsString = ingredientsString;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = _context.Categories.ToList();

            ViewBag.TopLiked = _context.Recipes.Include(r => r.Comments).Where(r => r.IsApproved == true).OrderByDescending(r => r.LuotLike).Take(5).ToList();
            ViewBag.TopCommented = _context.Recipes.Include(r => r.Comments).Where(r => r.IsApproved == true).OrderByDescending(r => r.Comments.Count).Take(5).ToList();

            var query = _context.Recipes.Include(r => r.Category).Include(r => r.Comments).AsQueryable();
            if (role != "Admin")
            {
                query = query.Where(r => r.IsApproved == true);
            }
            var recipes = query.ToList();

            foreach (var r in recipes)
            {
                if (!string.IsNullOrEmpty(r.HinhanhUrl) && !r.HinhanhUrl.StartsWith("/"))
                {
                    r.HinhanhUrl = "/" + r.HinhanhUrl;
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.TenMon.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(ingredientsString))
            {
                var keywords = ingredientsString.Split(',').Select(k => k.Trim().ToLower()).Where(k => !string.IsNullOrEmpty(k));
                recipes = recipes.Where(r => keywords.Any(k => r.NguyenLieu.ToLower().Contains(k))).ToList();
            }

            if (categoryId.HasValue)
            {
                recipes = recipes.Where(r => r.CategoryId == categoryId.Value).ToList();
            }

            return View(recipes);
        }

        // TÍNH NĂNG PHỤC HỒI: Xem chi tiết một công thức món ăn
        public IActionResult Details(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Comments)
                .FirstOrDefault(r => r.Id == id);

            if (recipe == null) return NotFound();

            // Sửa đường dẫn ảnh nếu cần
            if (!string.IsNullOrEmpty(recipe.HinhanhUrl) && !recipe.HinhanhUrl.StartsWith("/"))
            {
                recipe.HinhanhUrl = "/" + recipe.HinhanhUrl;
            }

            // Bảo mật: Nếu món chưa duyệt thì chỉ Admin hoặc chính chủ mới được xem
            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("Username");

            if (!recipe.IsApproved && role != "Admin" && recipe.Username != username)
            {
                return Content("Bài viết này đang chờ duyệt, bạn không có quyền xem!");
            }

            return View(recipe);
        }

        // ==========================================
        // 2. CÁC TÍNH NĂNG TƯƠNG TÁC (THÊM, SỬA, THÍCH, BÌNH LUẬN)
        // ==========================================

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Recipe recipe, IFormFile? Hinhanh)
        {
            var currentUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(currentUsername))
            {
                return RedirectToAction("Login", "Account");
            }

            if (Hinhanh != null && Hinhanh.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinhanh.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { Hinhanh.CopyTo(stream); }
                recipe.HinhanhUrl = "/images/" + fileName;
            }
            else { recipe.HinhanhUrl = "/images/default.jpg"; }

            recipe.Username = currentUsername;
            recipe.IsApproved = false;
            recipe.LuotLike = 0;

            _context.Recipes.Add(recipe);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            var recipe = _context.Recipes.Find(id);
            if (recipe == null) return NotFound();

            if (role != "Admin" && recipe.Username != username)
            {
                return Content("Bạn không có quyền chỉnh sửa bài viết này!");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        [HttpPost]
        public IActionResult Edit(int id, Recipe updatedModel, IFormFile? Hinhanh)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            var recipe = _context.Recipes.Find(id);
            if (recipe == null) return NotFound();

            if (role != "Admin" && recipe.Username != username) return Content("Không có quyền!");

            recipe.TenMon = updatedModel.TenMon;
            recipe.NguyenLieu = updatedModel.NguyenLieu;
            recipe.CongThuc = updatedModel.CongThuc;
            recipe.CategoryId = updatedModel.CategoryId;

            if (Hinhanh != null && Hinhanh.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinhanh.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { Hinhanh.CopyTo(stream); }
                recipe.HinhanhUrl = "/images/" + fileName;
            }

            if (role != "Admin")
            {
                recipe.IsApproved = false;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Like(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username"))) return RedirectToAction("Login", "Account");
            var monAn = _context.Recipes.Find(id);
            if (monAn != null) { monAn.LuotLike += 1; _context.SaveChanges(); }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Comment(int recipeId, string noiDung)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrEmpty(noiDung))
            {
                var newComment = new Comment { RecipeId = recipeId, Username = username, Role = role ?? "Member", NoiDung = noiDung, NgayDang = DateTime.Now };
                _context.Comments.Add(newComment); _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // 3. KHU VỰC QUẢN TRỊ ADMIN (DUYỆT, XÓA, THÊM DANH MỤC)
        // ==========================================

        public IActionResult AdminDuyet()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");

            var recipes = _context.Recipes.Include(r => r.Category).ToList();

            foreach (var r in recipes)
            {
                if (!string.IsNullOrEmpty(r.HinhanhUrl) && !r.HinhanhUrl.StartsWith("/"))
                {
                    r.HinhanhUrl = "/" + r.HinhanhUrl;
                }
            }

            return View(recipes);
        }

        public IActionResult Approve(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");

            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                recipe.IsApproved = true;
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDuyet");
        }

        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");

            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDuyet");
        }

        [HttpPost]
        public IActionResult UploadBackground(IFormFile BackgroundFile)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");

            if (BackgroundFile != null && BackgroundFile.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "bg_cooking.jpg");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BackgroundFile.CopyTo(stream);
                }
            }
            return RedirectToAction("AdminDuyet");
        }

        // TÍNH NĂNG PHỤC HỒI: Thêm danh mục mới (Chỉ Admin mới có quyền)
        public IActionResult CreateCategory()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}