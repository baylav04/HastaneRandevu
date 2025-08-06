using AutoMapper;
using HastaneRandevu.Data;
using HastaneRandevu.DTOs;
using HastaneRandevu.Filters;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace HastaneRandevu.Controllers
{
    public class HastasController : Controller
    {
        private readonly IMapper _mapper;
        private readonly Context _context;

        public HastasController(IMapper mapper, Context context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult SifremiUnuttum()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(HastaSifreDegistirDTO model)
        {
            if (!ModelState.IsValid)
                return View("SifremiUnuttum", model);

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(h => h.TCKimlikNo == model.TCKimlikNo);

            if (hasta == null)
            {
                ViewData["ErrorMessage"] = "Bu TC Kimlik numarası sistemde bulunamadı.";
                return View("SifremiUnuttum", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewData["ErrorMessage"] = "Girdiğiniz şifreler birbiriyle uyuşmuyor.";
                return View("SifremiUnuttum", model);
            }

            hasta.Parola = model.NewPassword;
            _context.Update(hasta);
            await _context.SaveChangesAsync();

            ViewData["SuccessMessage"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
            return View("SifremiUnuttum", new HastaSifreDegistirDTO());
        }

        [HastaAuthorize]
        public async Task<IActionResult> Index()
        {
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            var hastalar = await _context.Hastalar
                .Where(h => h.Id == loggedInHastaId)
                .ToListAsync();
            return View(hastalar);
        }

        [HastaAuthorize]
        public async Task<IActionResult> Profil(int? id)
        {
            if (id == null)
                return NotFound();

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
                return RedirectToAction("Profil", new { id = loggedInHastaId });

            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta == null)
                return NotFound();

            var dto = _mapper.Map<HastaDTO>(hasta);
            return View(dto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyadi,Parola,TCKimlikNo")] Hasta hasta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hasta);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("LoggedInHastaId", hasta.Id);
                HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);
                return RedirectToAction("Index", "Home");
            }
            return View(hasta);
        }

        [HastaAuthorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
                return RedirectToAction("Edit", new { id = loggedInHastaId });

            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta == null)
                return NotFound();

            return View(hasta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyadi,Parola,TCKimlikNo")] Hasta hasta)
        {
            if (id != hasta.Id)
                return NotFound();

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hasta);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HastaExists(hasta.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(hasta);
        }

        [HastaAuthorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
                return RedirectToAction("Index", "Home");

            var hasta = await _context.Hastalar.FirstOrDefaultAsync(m => m.Id == id);
            if (hasta == null)
                return NotFound();

            return View(hasta);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ShowValidation"] = false;
            ViewData["HealthAlert"] = "⚠️ Grip mevsimi başladı, lütfen maskenizi takmayı unutmayın!";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Hasta model)
        {
            // 1. reCAPTCHA kontrolü
            var captchaResponse = Request.Form["g-recaptcha-response"];
            using var client = new HttpClient();
            var secretKey = "6Lc3XpsrAAAAALTf1A_PmGCOptRkHal7cx3Hohg_"; // kendi anahtarını koy
            var verifyResponse = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}", null);

            var responseString = await verifyResponse.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseString);
            var isSuccess = json.RootElement.GetProperty("success").GetBoolean();

            if (!isSuccess)
            {
                ViewData["LoginError"] = "Lütfen robot olmadığınızı doğrulayın.";
                return View(model);
            }

            // 2. Kullanıcı bilgisi kontrolü
            if (string.IsNullOrWhiteSpace(model.Parola) || string.IsNullOrWhiteSpace(model.TCKimlikNo))
            {
                ViewData["LoginError"] = "Lütfen tüm alanları doldurun.";
                return View(model);
            }

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(h => h.Parola == model.Parola && h.TCKimlikNo == model.TCKimlikNo);

            if (hasta == null)
            {
                ViewData["LoginError"] = "Parola veya TC Kimlik No hatalı.";
                return View(model);
            }

            // 3. Oturum bilgilerini kaydet
            HttpContext.Session.SetInt32("LoggedInHastaId", hasta.Id);
            HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);

            return RedirectToAction("HastaRandevulari", "Randevus", new { hastaId = hasta.Id });
        }

        public IActionResult Logout()
        {
            // Hasta oturumunu temizle
            HttpContext.Session.Clear();

            // Eğer admin girişi varsa etkilenmez (Identity ayrı)
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
                return RedirectToAction("Index", "Home");

            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta != null)
            {
                _context.Hastalar.Remove(hasta);
                await _context.SaveChangesAsync();
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Index", "Home");
        }

        private bool HastaExists(int id)
        {
            return _context.Hastalar.Any(e => e.Id == id);
        }
    }
}

