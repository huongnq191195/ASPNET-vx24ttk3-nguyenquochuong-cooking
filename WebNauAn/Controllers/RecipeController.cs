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
        public RecipeController(AppDbContext context) { _context = context; }

        private string LoaiBoDauTiengViet(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            text = text.ToLower().Trim();
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ" };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y" };
            for (int i = 0; i < arr1.Length; i++) { text = text.Replace(arr1[i], arr2[i]); }
            return text;
        }

        public IActionResult Index(string searchString, string ingredientsString, int? categoryId, int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            ViewBag.SearchString = searchString;
            ViewBag.IngredientsString = ingredientsString;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = _context.Categories.ToList();

            ViewBag.TopLiked = _context.Recipes.Include(r => r.Comments).Where(r => r.IsApproved == true && r.LuotLike > 0).OrderByDescending(r => r.LuotLike).Take(3).ToList();
            ViewBag.TopCommented = _context.Recipes.Include(r => r.Comments).Where(r => r.IsApproved == true && r.Comments.Count > 0).OrderByDescending(r => r.Comments.Count).Take(3).ToList();

            var query = _context.Recipes.Include(r => r.Category).Include(r => r.Comments).AsQueryable();
            if (role != "Admin") { query = query.Where(r => r.IsApproved == true); }
            var recipes = query.ToList();

            foreach (var r in recipes)
            {
                if (!string.IsNullOrEmpty(r.HinhanhUrl) && !r.HinhanhUrl.StartsWith("/")) r.HinhanhUrl = "/" + r.HinhanhUrl;
            }

            if (!string.IsNullOrEmpty(searchString)) { recipes = recipes.Where(r => r.TenMon.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); }
            if (!string.IsNullOrEmpty(ingredientsString))
            {
                var keywords = ingredientsString.Split(',').Select(k => k.Trim().ToLower()).Where(k => !string.IsNullOrEmpty(k));
                recipes = recipes.Where(r => keywords.Any(k => r.NguyenLieu.ToLower().Contains(k))).ToList();
            }
            if (categoryId.HasValue) { recipes = recipes.Where(r => r.CategoryId == categoryId.Value).ToList(); }

            int pageSize = 10;
            int totalItems = recipes.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedRecipes = recipes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.UserLikes = _context.UserLikes.Where(u => u.Username == username).Select(u => u.RecipeId).ToList();
                ViewBag.UserSaved = _context.SavedRecipes.Where(u => u.Username == username).Select(u => u.RecipeId).ToList();
            }
            else { ViewBag.UserLikes = new List<int>(); ViewBag.UserSaved = new List<int>(); }

            return View(pagedRecipes);
        }

        public IActionResult Details(int id)
        {
            var recipe = _context.Recipes.Include(r => r.Category).Include(r => r.Comments).FirstOrDefault(r => r.Id == id);
            if (recipe == null) return NotFound();
            if (!string.IsNullOrEmpty(recipe.HinhanhUrl) && !recipe.HinhanhUrl.StartsWith("/")) recipe.HinhanhUrl = "/" + recipe.HinhanhUrl;

            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("Username");

            if (!recipe.IsApproved && role != "Admin" && recipe.Username != username) { return Content("Bài viết này đang chờ duyệt, bạn không có quyền xem!"); }

            ViewBag.IsLiked = false; ViewBag.IsSaved = false;
            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.IsLiked = _context.UserLikes.Any(l => l.RecipeId == id && l.Username == username);
                ViewBag.IsSaved = _context.SavedRecipes.Any(s => s.RecipeId == id && s.Username == username);
            }

            return View(recipe);
        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username"))) return RedirectToAction("Login", "Account");
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Recipe recipe, IFormFile? Hinhanh)
        {
            var currentUsername = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(currentUsername)) return RedirectToAction("Login", "Account");

            if (Hinhanh != null && Hinhanh.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinhanh.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { Hinhanh.CopyTo(stream); }
                recipe.HinhanhUrl = "/images/" + fileName;
            }
            else { recipe.HinhanhUrl = "/images/default.jpg"; }

            recipe.Username = currentUsername;
            recipe.IsApproved = (role == "Admin");
            recipe.LuotLike = 0;
            _context.Recipes.Add(recipe); _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var recipe = _context.Recipes.Find(id);
            if (recipe == null) return NotFound();
            if (role != "Admin" && recipe.Username != username) return Content("Bạn không có quyền chỉnh sửa bài viết này!");
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

            if (role != "Admin")
            {
                if (recipe.IsApproved == true)
                {
                    recipe.TenMonCu = recipe.TenMon;
                    recipe.NguyenLieuCu = recipe.NguyenLieu;
                    recipe.CongThucCu = recipe.CongThuc;
                    recipe.IsSuaDoi = true;
                }
                recipe.IsApproved = false;
            }

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

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = recipe.Id });
        }

        public IActionResult Like(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                var existingLike = _context.UserLikes.FirstOrDefault(l => l.RecipeId == id && l.Username == username);
                if (existingLike != null) { _context.UserLikes.Remove(existingLike); recipe.LuotLike--; }
                else { _context.UserLikes.Add(new UserLike { RecipeId = id, Username = username }); recipe.LuotLike++; }
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Recipe/Index");
        }

        public IActionResult SaveRecipe(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var existingSave = _context.SavedRecipes.FirstOrDefault(s => s.RecipeId == id && s.Username == username);
            if (existingSave != null) { _context.SavedRecipes.Remove(existingSave); }
            else { _context.SavedRecipes.Add(new SavedRecipe { RecipeId = id, Username = username }); }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Recipe/Index");
        }

        public IActionResult SavedList()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var savedRecipes = _context.SavedRecipes.Include(s => s.Recipe).ThenInclude(r => r.Category).Where(s => s.Username == username).Select(s => s.Recipe).ToList();
            foreach (var r in savedRecipes) { if (!string.IsNullOrEmpty(r.HinhanhUrl) && !r.HinhanhUrl.StartsWith("/")) r.HinhanhUrl = "/" + r.HinhanhUrl; }
            return View(savedRecipes);
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
            // ĐÃ SỬA: CHUYỂN VỀ TRANG CHI TIẾT SAU KHI COMMENT
            return RedirectToAction("Details", new { id = recipeId });
        }

        public IActionResult DeleteComment(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin")
            {
                var cmt = _context.Comments.Find(id);
                if (cmt != null) { _context.Comments.Remove(cmt); _context.SaveChanges(); }
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Recipe/Index");
        }

        public IActionResult AdminDuyet()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");
            var recipes = _context.Recipes.Include(r => r.Category).ToList();
            foreach (var r in recipes) { if (!string.IsNullOrEmpty(r.HinhanhUrl) && !r.HinhanhUrl.StartsWith("/")) r.HinhanhUrl = "/" + r.HinhanhUrl; }
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
                recipe.IsSuaDoi = false;
                recipe.TenMonCu = null;
                recipe.NguyenLieuCu = null;
                recipe.CongThucCu = null;
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Recipe/AdminDuyet");
        }

        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Index", "Recipe");
            var recipe = _context.Recipes.Find(id);
            if (recipe != null) { _context.Recipes.Remove(recipe); _context.SaveChanges(); }
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
                using (var stream = new FileStream(filePath, FileMode.Create)) { BackgroundFile.CopyTo(stream); }
            }
            return RedirectToAction("AdminDuyet");
        }

        public IActionResult CreateCategory() { return View(); }

        [HttpPost]
        public IActionResult CreateCategory(string MucCha, string MucCon)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") { TempData["ErrorMessage"] = "Lỗi: Bạn không có quyền thực hiện thao tác này!"; return RedirectToAction("Index"); }
            if (string.IsNullOrWhiteSpace(MucCha) || string.IsNullOrWhiteSpace(MucCon)) { TempData["ErrorMessage"] = "Lỗi: Vui lòng điền đầy đủ thông tin danh mục!"; return RedirectToAction("Create"); }
            MucCha = MucCha.Trim(); MucCon = MucCon.Trim();
            string mucChaKhongDau = LoaiBoDauTiengViet(MucCha); string mucConKhongDau = LoaiBoDauTiengViet(MucCon);
            var allCategories = _context.Categories.ToList();
            var isDuplicate = allCategories.Any(c => LoaiBoDauTiengViet(c.MucCha) == mucChaKhongDau && LoaiBoDauTiengViet(c.MucCon) == mucConKhongDau);
            if (isDuplicate) { TempData["ErrorMessage"] = "Lỗi, danh mục đã có sẵn!"; return RedirectToAction("Create"); }
            var existingParent = allCategories.FirstOrDefault(c => LoaiBoDauTiengViet(c.MucCha) == mucChaKhongDau);
            if (existingParent != null) { MucCha = existingParent.MucCha; }
            var category = new Category { MucCha = MucCha, MucCon = MucCon };
            _context.Categories.Add(category); _context.SaveChanges();
            TempData["SuccessMessage"] = "Thêm danh mục thành công, mời bạn đăng món!";
            return RedirectToAction("Create");
        }
    }
}