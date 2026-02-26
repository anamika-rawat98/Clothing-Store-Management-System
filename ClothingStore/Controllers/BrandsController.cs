using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothingStore.Data;
using ClothingStore.Models;

namespace ClothingStore.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Brands
        public IActionResult Index()
        {
            var brands = _context.Brands.ToList();
            return View(brands);
        }

        // GET: Brands/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var brand = _context.Brands
                .FirstOrDefault(b => b.BrandId == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);

            _context.Brands.Add(brand);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Brand added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var brand = _context.Brands.Find(id);
            if (brand == null) return NotFound();

            return View(brand);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Brand brand)
        {
            if (id != brand.BrandId) return NotFound();

            if (!ModelState.IsValid) return View(brand);

            try
            {
                _context.Update(brand);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.BrandId)) return NotFound();
                else throw;
            }

            TempData["SuccessMessage"] = "Brand updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var brand = _context.Brands
                .FirstOrDefault(b => b.BrandId == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var brand = _context.Brands.Find(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "Brand deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(b => b.BrandId == id);
        }
    }
}
