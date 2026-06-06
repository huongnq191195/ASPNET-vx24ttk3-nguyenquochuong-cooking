using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using WebNauAn.Models;

namespace WebNauAn.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            // 1. Kiểm tra xem tên tài khoản đã có ai dùng chưa
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Tên tài khoản này đã có người sử dụng rồi bạn ơi!");
                return View(user);
            }

            // 2. Tự động phân quyền ngầm
            if (user.Username?.ToLower() == "admin")
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "Member";
            }

            // 3. Lưu vào Database
            _context.Users.Add(user);
            _context.SaveChanges();

            // 4. Bắn thông báo thành công sang trang Login
            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Đăng nhập thử liền đi bạn.";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Tìm chính xác user và password trong DB
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Lưu thông tin vào Session để các trang khác kiểm tra quyền
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);

                return RedirectToAction("Index", "Recipe");
            }

            // Nếu sai tài khoản hoặc mật khẩu
            ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác, kiểm tra lại nha!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa sạch phiên đăng nhập
            return RedirectToAction("Index", "Recipe");
        }
    }
}