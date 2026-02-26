using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClothingStore.Data;
using ClothingStore.Models;

namespace ClothingStore.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderItemId == id);

            if (orderItem == null) return NotFound();

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = _context.Orders
                .Include(o => o.Customer)
                .Select(o => new SelectListItem
                {
                    Value = o.OrderId.ToString(),
                    Text = $"Order #{o.OrderId} - {o.Customer.FullName}"
                })
                .ToList();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");

            return View();
        }

        // POST: OrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,ProductId,Quantity")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = _context.Orders
                .Include(o => o.Customer)
                .Select(o => new SelectListItem
                {
                    Value = o.OrderId.ToString(),
                    Text = $"Order #{o.OrderId} - {o.Customer.FullName}"
                })
                .ToList();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderItem.ProductId);

            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            ViewData["OrderId"] = _context.Orders
                .Include(o => o.Customer)
                .Select(o => new SelectListItem
                {
                    Value = o.OrderId.ToString(),
                    Text = $"Order #{o.OrderId} - {o.Customer.FullName}"
                })
                .ToList();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderItem.ProductId);

            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderItemId,OrderId,ProductId,Quantity")] OrderItem orderItem)
        {
            if (id != orderItem.OrderItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.OrderItemId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = _context.Orders
                .Include(o => o.Customer)
                .Select(o => new SelectListItem
                {
                    Value = o.OrderId.ToString(),
                    Text = $"Order #{o.OrderId} - {o.Customer.FullName}"
                })
                .ToList();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderItem.ProductId);

            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderItemId == id);

            if (orderItem == null) return NotFound();

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}