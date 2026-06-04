using Microsoft.AspNetCore.Mvc;

namespace WebNauAn.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
