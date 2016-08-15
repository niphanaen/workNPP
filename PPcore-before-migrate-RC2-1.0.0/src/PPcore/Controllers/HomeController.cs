using Microsoft.AspNetCore.Mvc;
using PPcore.Models;

namespace PPcore.Controllers
{
    public class HomeController : Controller
    {
        private PalangPanyaDBContext _context;

        public HomeController(PalangPanyaDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
