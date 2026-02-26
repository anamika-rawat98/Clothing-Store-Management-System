using ClothingStore.Data;
using ClothingStore.Models;
using ClothingStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClothingStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            var vm = new ProductVM
            {
                Brands = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.Name
                }).ToList(),
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(vm);
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Brands = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.Name
                }).ToList();
                vm.Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();
                return View(vm);
            }

            var product = new Product
            {
                Name = vm.Name,
                Price = vm.Price,
                ImageUrl = NormalizeImageUrl(vm.ImageUrl),
                Color = vm.Color,
                BrandId = vm.BrandId,
                CategoryId = vm.CategoryId
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product added successfully!";
            return RedirectToAction("Create");
        }

        // EDIT - GET
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var vm = new ProductVM
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Color = product.Color,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                Brands = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.Name
                }).ToList(),
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(vm);
        }

        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Brands = _context.Brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.Name
                }).ToList();
                vm.Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();
                return View(vm);
            }

            var product = _context.Products.Find(vm.ProductId);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Price = vm.Price;
            product.ImageUrl = NormalizeImageUrl(vm.ImageUrl);
            product.Color = vm.Color;
            product.BrandId = vm.BrandId;
            product.CategoryId = vm.CategoryId;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("Index");
        }

        // DETAILS - GET
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();
            return View(product);
        }

        // DELETE - GET
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }

        private static string NormalizeImageUrl(string imageUrl)
        {
            var trimmed = (imageUrl ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return string.Empty;
            }

            if (trimmed.StartsWith("http://") || trimmed.StartsWith("https://") || trimmed.StartsWith("/"))
            {
                return trimmed;
            }

            return $"/images/{trimmed}";
        }
    }
}
