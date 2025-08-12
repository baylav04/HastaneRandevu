using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Models;
using HastaneRandevu.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HastaneRandevu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly Context _context;

        public AdminController(Context context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

        [HttpPost]
        public async Task<IActionResult> SendEmail(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doldurun.");
                return View("Index");
            }
            try
            {
                await _emailSender.SendEmailAsync(toEmail, subject, message);
                ViewBag.SuccessMessage = "E-posta başarıyla gönderildi.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"E-posta gönderilirken bir hata oluştu: {ex.Message}");
            }
            return View("Index");
        }
    }
}


