using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Data;
using HastaneRandevu.Models;
using HastaneRandevu.Filters;

namespace HastaneRandevu.Controllers
{
    public class RandevusController : Controller
    {
        private readonly Context _context;

        public RandevusController(Context context)
        {
            _context = context;
        }

        [HastaAuthorize]
        public async Task<IActionResult> Index()
        {
            var context = _context.Randevular
                .Include(r => r.Doktor)
                .Include(r => r.Hasta)
                .Include(r => r.Poliklinik);
            return View(await context.ToListAsync());
        }

        [HastaAuthorize]
        public async Task<IActionResult> HastaRandevulari(int hastaId)
        {
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");

            if (loggedInHastaId != hastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            var randevular = await _context.Randevular
                .Include(r => r.Poliklinik)
                .Include(r => r.Doktor)
                .Where(r => r.HastaId == hastaId)
                .ToListAsync();

            return View(randevular);
        }

        [HastaAuthorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Doktor)
                .Include(r => r.Hasta)
                .Include(r => r.Poliklinik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null)
            {
                return NotFound();
            }

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (loggedInHastaId != randevu.HastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            return View(randevu);
        }

        [HttpGet]
        public JsonResult DoktorGetir(int poliklinikId)
        {
            var doktorlar = _context.Doktorlar
                .Where(d => d.PoliklinikId == poliklinikId)
                .Select(d => new
                {
                    d.Id,
                    d.Ad
                })
                .ToList();

            return Json(doktorlar);
        }

        // GET: Randevus/Create
        [HastaAuthorize]
        public IActionResult Create()
        {
            var hastaId = HttpContext.Session.GetInt32("LoggedInHastaId");

            ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad");
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi");

            var model = new Randevu
            {
                HastaId = hastaId ?? 0
            };

            return View(model);
        }

        // POST: Randevus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> Create([Bind("Id,PoliklinikId,DoktorId,HastaId,RandevuSaati")] Randevu randevu)
        {
            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");

            if (loggedInHastaId != randevu.HastaId)
            {
                ModelState.AddModelError("", "Geçersiz hasta bilgisi.");
                ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
                ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
                return View(randevu);
            }

            // Aynı doktor ve aynı saatte randevu var mı kontrolü
            bool doktorRandevuVarMi = await _context.Randevular.AnyAsync(r =>
                r.DoktorId == randevu.DoktorId &&
                r.RandevuSaati == randevu.RandevuSaati);

            if (doktorRandevuVarMi)
            {
                ModelState.AddModelError("", "Bu saatte doktorun başka bir randevusu var.");
                ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
                ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
                return View(randevu);
            }

            // Aynı hasta ve aynı saatte başka bir randevusu var mı kontrolü
            bool hastaRandevuVarMi = await _context.Randevular.AnyAsync(r =>
                r.HastaId == randevu.HastaId &&
                r.RandevuSaati == randevu.RandevuSaati);

            if (hastaRandevuVarMi)
            {
                ModelState.AddModelError("", "Bu saatte zaten bir randevunuz mevcut.");
                ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
                ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
                return View(randevu);
            }

            if (ModelState.IsValid)
            {
                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction("HastaRandevulari", new { hastaId = loggedInHastaId });
            }

            ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
            return View(randevu);
        }


        // GET: Randevus/Edit/5
        [HastaAuthorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (loggedInHastaId != randevu.HastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
            return View(randevu);
        }

        // POST: Randevus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PoliklinikId,DoktorId,HastaId,RandevuSaati")] Randevu randevu)
        {
            if (id != randevu.Id)
            {
                return NotFound();
            }

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (loggedInHastaId != randevu.HastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            // Hasta aynı saatte başka randevusu var mı kontrolü (kendi kaydını hariç tut)
            bool hastaAyniSaatRandevuVarMi = await _context.Randevular.AnyAsync(r =>
                r.HastaId == randevu.HastaId &&
                r.RandevuSaati == randevu.RandevuSaati &&
                r.Id != randevu.Id);

            if (hastaAyniSaatRandevuVarMi)
            {
                ModelState.AddModelError("", "Bu saatte zaten bir randevunuz mevcut. Lütfen farklı bir saat seçiniz.");
                ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
                ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
                return View(randevu);
            }

            // Doktorun aynı saatte başka randevusu var mı (Edit için ekleyebiliriz, istersen)
            bool doktorAyniSaatRandevuVarMi = await _context.Randevular.AnyAsync(r =>
                r.DoktorId == randevu.DoktorId &&
                r.RandevuSaati == randevu.RandevuSaati &&
                r.Id != randevu.Id);

            if (doktorAyniSaatRandevuVarMi)
            {
                ModelState.AddModelError("", "Bu saatte doktorun başka bir randevusu var.");
                ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
                ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
                return View(randevu);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RandevuExists(randevu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("HastaRandevulari", new { hastaId = loggedInHastaId });
            }

            ViewData["DoktorId"] = new SelectList(_context.Doktorlar, "Id", "Ad", randevu.DoktorId);
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", randevu.PoliklinikId);
            return View(randevu);
        }

        // GET: Randevus/Delete/5
        [HastaAuthorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Doktor)
                .Include(r => r.Hasta)
                .Include(r => r.Poliklinik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null)
            {
                return NotFound();
            }

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (loggedInHastaId != randevu.HastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            return View(randevu);
        }

        // POST: Randevus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HastaAuthorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            var loggedInHastaId = HttpContext.Session.GetInt32("LoggedInHastaId");
            if (loggedInHastaId != randevu.HastaId)
            {
                return RedirectToAction("Login", "Hastas");
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction("HastaRandevulari", new { hastaId = loggedInHastaId });
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}

