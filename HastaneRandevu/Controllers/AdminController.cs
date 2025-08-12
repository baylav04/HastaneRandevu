using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Models;
using HastaneRandevu.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HastaneRandevu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly INotyfService _toastNotification;
        private readonly IEmailSender _emailSender;
        private readonly Context _context;

        public AdminController(Context context, IEmailSender emailSender, INotyfService notyfService)
        {
            _context = context;
            _emailSender = emailSender;
            _toastNotification = notyfService;
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
            try
            {
                if (string.IsNullOrEmpty(toEmail))
                {
                    return Json(new { success = false, toastType = "error", toastMessage = "Mail alıcı adresi boş olamaz." });
                }

                await _emailSender.SendEmailAsync(toEmail, subject, message);

                return Json(new { success = true, toastType = "success", toastMessage = $"Mail {toEmail} adresine gönderildi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, toastType = "error", toastMessage = $"Mail gönderilirken hata oluştu: {ex.Message}" });
            }
        }
    }
}


