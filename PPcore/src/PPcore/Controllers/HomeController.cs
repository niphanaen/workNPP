using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPcore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

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
            return RedirectToAction(nameof(membersController.Index), "members");
        }
    }
}
