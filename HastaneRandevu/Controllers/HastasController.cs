using AutoMapper;
using HastaneRandevu.Data;
using HastaneRandevu.DTOs;
using HastaneRandevu.Filters;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            {
                return View("SifremiUnuttum", model);
            }

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(h => h.TCKimlikNo == model.TCKimlikNo);

            if (hasta == null)
            {
                ViewData["ErrorMessage"] = "Bu TC Kimlik numarası sistemde bulunamadı.";
                return View("SifremiUnuttum", model);
            }

            // Şifre uyumluluğunu kontrol et
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewData["ErrorMessage"] = "Girdiğiniz şifreler birbiriyle uyuşmuyor.";
                return View("SifremiUnuttum", model);
            }

            // Şifreyi güncelle
            hasta.Parola = model.NewPassword;
            _context.Update(hasta);
            await _context.SaveChangesAsync();

            ViewData["SuccessMessage"] = "Şifreniz başarıyla güncellenmiştir. Yeni şifrenizle giriş yapabilirsiniz.";
            return View("SifremiUnuttum", new HastaSifreDegistirDTO());
        }
        
        // GET: Hastas
        [HastaAuthorize]
        public async Task<IActionResult> Index()
        {
            // Oturum açan kullanıcı admin değilse sadece kendi bilgilerini görebilsin
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            var loggedInHasta = await _context.Hastalar.FindAsync(loggedInHastaId);
            
            // Admin kontrolü (gerçek uygulamada rol tabanlı bir yaklaşım kullanılabilir)
            // Şimdilik basit bir çözüm olarak admin olmadığını varsayıyoruz
            // ve her kullanıcının sadece kendi bilgilerini görmesini sağlıyoruz
            
            var hastalar = await _context.Hastalar
                .Where(h => h.Id == loggedInHastaId)
                .ToListAsync();
                
            return View(hastalar);
        }

        // GET: Hastas/Details/5
        [HastaAuthorize]
        public async Task<IActionResult> Profil(int? id)
        {
            if (id == null)
                return NotFound(); // URL'de ID yoksa 404 döner

            // Giriş yapan kullanıcının kendi profilini görmesini sağla
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
            {
                return RedirectToAction("Profil", new { id = loggedInHastaId });
            }

            // Veritabanından hasta bilgisi çek
            var hasta = await _context.Hastalar.FindAsync(id);

            if (hasta == null)
                return NotFound(); // ID'ye karşılık hasta yoksa 404 döner

            // Model → DTO dönüşümü yap
            var dto = _mapper.Map<HastaDTO>(hasta);

            // View sayfasına DTO'yu gönder
            return View(dto);
        }

        // GET: Hastas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hastas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyadi,Parola,TCKimlikNo")] Hasta hasta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hasta);
                await _context.SaveChangesAsync();
                
                // Kayıt başarılı olunca kullanıcıyı otomatik giriş yap
                HttpContext.Session.SetInt32("LoggedInHastaId", hasta.Id);
                HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);
                
                return RedirectToAction("Index", "Home");
            }
            return View(hasta);
        }

        // GET: Hastas/Edit/5
        [HastaAuthorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Sadece kendi profilini düzenlemeye izin ver
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
            {
                return RedirectToAction("Edit", new { id = loggedInHastaId });
            }

            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta == null)
            {
                return NotFound();
            }
            return View(hasta);
        }

        // POST: Hastas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyadi,Parola,TCKimlikNo")] Hasta hasta)
        {
            if (id != hasta.Id)
            {
                return NotFound();
            }

            // Sadece kendi profilini düzenlemeye izin ver
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hasta);
                    await _context.SaveChangesAsync();
                    
                    // Bilgileri güncelleyince session'daki bilgileri de güncelle
                    HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HastaExists(hasta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(hasta);
        }

        // GET: Hastas/Delete/5
        [HastaAuthorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Sadece kendi hesabını silmeye izin ver
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
            {
                return RedirectToAction("Index", "Home");
            }

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hasta == null)
            {
                return NotFound();
            }

            return View(hasta);
        }


        [HttpGet]
        public IActionResult Login()
        {
            // İlk kez sayfa açıldığında hata mesajı gösterilmesin
            ViewData["ShowValidation"] = false;

            // Sağlık uyarısı
            ViewData["HealthAlert"] = "⚠️ Grip mevsimi başladı, lütfen maskenizi takmayı unutmayın!";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Hasta model)
        {
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

            // Oturum bilgilerini kaydet
            HttpContext.Session.SetInt32("LoggedInHastaId", hasta.Id);
            HttpContext.Session.SetString("LoggedInHastaName", hasta.AdSoyadi);

            return RedirectToAction("HastaRandevulari", "Randevus", new { hastaId = hasta.Id });
        }

        // Çıkış işlemi
        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();
            
            // Ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

        // POST: Hastas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Sadece kendi hesabını silmeye izin ver
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (id != loggedInHastaId)
            {
                return RedirectToAction("Index", "Home");
            }
            
            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta != null)
            {
                _context.Hastalar.Remove(hasta);
                await _context.SaveChangesAsync();
                
                // Hesap silinince oturumu kapat
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
