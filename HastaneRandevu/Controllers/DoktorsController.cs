using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Data;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace HastaneRandevu.Controllers
{
    public class DoktorsController : Controller
    {
        private readonly Context _context;

        public DoktorsController(Context context)
        {
            _context = context;
        }

        // Doktor listesi ve filtreleme
        public async Task<IActionResult> Index(string brans, string arama)
        {
            var doktorlar = _context.Doktorlar.Include(d => d.Poliklinik).AsQueryable();

            if (!string.IsNullOrEmpty(brans))
                doktorlar = doktorlar.Where(d => d.Poliklinik.PoliklinikAdi == brans);

            if (!string.IsNullOrEmpty(arama))
                doktorlar = doktorlar.Where(d => d.Ad.Contains(arama));

            var model = new DoktorListViewModel
            {
                Doktorlar = await doktorlar.ToListAsync(),
                Branslar = await _context.Poliklinikler.Select(p => p.PoliklinikAdi).Distinct().ToListAsync(),
                SeciliBrans = brans,
                Arama = arama
            };

            return View(model);
        }

        // Doktor oluşturma GET
        public IActionResult Create()
        {
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi");
            ViewBag.Doktorlar = _context.Doktorlar.Include(d => d.Poliklinik).ToList();
            return View();
        }

        // Doktor oluşturma POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad,PoliklinikId")] Doktor doktor, IFormFile Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null && Foto.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doktorlar");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    var dosyaAdi = Guid.NewGuid() + Path.GetExtension(Foto.FileName);
                    var dosyaYolu = Path.Combine(uploads, dosyaAdi);

                    using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                        await Foto.CopyToAsync(stream);

                    doktor.FotoUrl = dosyaAdi;
                }

                _context.Add(doktor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", doktor.PoliklinikId);
            ViewBag.Doktorlar = _context.Doktorlar.Include(d => d.Poliklinik).ToList();
            return View(doktor);
        }

        // Doktor düzenleme GET - sadece seçilen doktoru getirir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doktor = await _context.Doktorlar.FindAsync(id);
            if (doktor == null) return NotFound();

            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", doktor.PoliklinikId);
            return View(doktor);
        }

        // Doktor düzenleme POST - formdan gelen bilgileri ve fotoğrafı günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,PoliklinikId,FotoUrl")] Doktor doktor, IFormFile YeniFoto)
        {
            if (id != doktor.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var mevcutDoktor = await _context.Doktorlar.FindAsync(id);
                    if (mevcutDoktor == null)
                        return NotFound();

                    mevcutDoktor.Ad = doktor.Ad;
                    mevcutDoktor.PoliklinikId = doktor.PoliklinikId;

                    if (YeniFoto != null && YeniFoto.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "doktorlar");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        // Eski fotoğrafı sil
                        if (!string.IsNullOrEmpty(mevcutDoktor.FotoUrl))
                        {
                            var eskiFotoPath = Path.Combine(uploadsFolder, mevcutDoktor.FotoUrl);
                            if (System.IO.File.Exists(eskiFotoPath))
                                System.IO.File.Delete(eskiFotoPath);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(YeniFoto.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await YeniFoto.CopyToAsync(stream);
                        }

                        mevcutDoktor.FotoUrl = fileName;
                    }

                    _context.Update(mevcutDoktor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Doktorlar.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PoliklinikId"] = new SelectList(_context.Poliklinikler, "Id", "PoliklinikAdi", doktor.PoliklinikId);
            return View(doktor);
        }

        // Doktor detayları
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var doktor = await _context.Doktorlar
                .Include(d => d.Poliklinik)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doktor == null) return NotFound();

            return View(doktor);
        }

        // Doktor silme GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var doktor = await _context.Doktorlar
                .Include(d => d.Poliklinik)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doktor == null) return NotFound();

            return View(doktor);
        }

        // Doktor silme POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doktor = await _context.Doktorlar.FindAsync(id);
            if (doktor != null)
            {
                if (!string.IsNullOrEmpty(doktor.FotoUrl))
                {
                    var yol = Path.Combine("wwwroot/images/doktorlar", doktor.FotoUrl);
                    if (System.IO.File.Exists(yol))
                        System.IO.File.Delete(yol);
                }

                _context.Doktorlar.Remove(doktor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Yeni doktor oluşturma ekranından silme (isteğe bağlı)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromCreate(int id)
        {
            var doktor = await _context.Doktorlar.FindAsync(id);
            if (doktor != null)
            {
                if (!string.IsNullOrEmpty(doktor.FotoUrl))
                {
                    var yol = Path.Combine("wwwroot/images/doktorlar", doktor.FotoUrl);
                    if (System.IO.File.Exists(yol))
                        System.IO.File.Delete(yol);
                }

                _context.Doktorlar.Remove(doktor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Create));
        }
    }
}

