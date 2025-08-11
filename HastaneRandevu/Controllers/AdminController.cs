using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var hastalar = _context.Hastalar
                .Include(h => h.Randevular)
                    .ThenInclude(r => r.Doktor)
                        .ThenInclude(d => d.Poliklinik)
                .ToList();

            var viewModel = hastalar.Select(h => new HastaPoliklinikViewModel
            {
                AdSoyad = h.AdSoyadi,
                TcNo = h.TCKimlikNo,
                Poliklinikler = h.Randevular?
                    .Where(r => r.Doktor != null && r.Doktor.Poliklinik != null)
                    .Select(r => r.Doktor.Poliklinik.PoliklinikAdi)
                    .Distinct()
                    .ToList() ?? new List<string>()
            }).ToList();

            return View(viewModel);
        }
    }
}


