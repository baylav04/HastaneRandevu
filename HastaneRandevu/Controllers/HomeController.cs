using System.Diagnostics;
using HastaneRandevu.Data;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Context _context;

        public HomeController(ILogger<HomeController> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Hasta hasta)
        {
            var girisYapan = await _context.Hastalar
                .FirstOrDefaultAsync(h => h.KullaniciAdi == hasta.KullaniciAdi && h.TCKimlikNo == hasta.TCKimlikNo);

            if (girisYapan != null)
            {
                return RedirectToAction("Index", "Doktors");
            }

            ViewBag.Hata = "Kullanıcı adı veya TC Kimlik No hatalı.";
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Doktor()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}