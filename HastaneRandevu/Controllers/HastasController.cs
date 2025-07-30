using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Data;
using HastaneRandevu.Models;

namespace HastaneRandevu.Controllers
{
    public class HastasController : Controller
    {
        private readonly Context _context;

        public HastasController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SifremiUnuttum()
        {
            return View();
        }
        // GET: Hastas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hastalar.ToListAsync());
        }

        // GET: Hastas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hasta == null)
            {
                return NotFound();
            }

            return View(hasta);
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
        public async Task<IActionResult> Create([Bind("Id,AdSoyadi,KullaniciAdi,TCKimlikNo")] Hasta hasta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hasta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hasta);
        }

        // GET: Hastas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyadi,KullaniciAdi,TCKimlikNo")] Hasta hasta)
        {
            if (id != hasta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hasta);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(hasta);
        }

        // GET: Hastas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
            if (string.IsNullOrWhiteSpace(model.KullaniciAdi) || string.IsNullOrWhiteSpace(model.TCKimlikNo))
            {
                ViewData["LoginError"] = "Lütfen tüm alanları doldurun.";
                return View(model);
            }

            var hasta = await _context.Hastalar
                .FirstOrDefaultAsync(h => h.KullaniciAdi == model.KullaniciAdi && h.TCKimlikNo == model.TCKimlikNo);

            if (hasta == null)
            {
                ViewData["LoginError"] = "Kullanıcı adı veya TC Kimlik No hatalı.";
                return View(model);
            }

            return RedirectToAction("HastaRandevulari", "Randevus", new { hastaId = hasta.Id });
        }
        // POST: Hastas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasta = await _context.Hastalar.FindAsync(id);
            if (hasta != null)
            {
                _context.Hastalar.Remove(hasta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HastaExists(int id)
        {
            return _context.Hastalar.Any(e => e.Id == id);
        }
    }
}
