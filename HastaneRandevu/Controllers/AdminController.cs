using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HastaneRandevu.Models;
using HastaneRandevu.Data;
using System.Linq;

namespace HastaneRandevu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Context _context;

        public AdminController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult HastaListesi()
        {
            var hastalar = _context.Hastalar.ToList();
            return View(hastalar);
        }
    }
}
