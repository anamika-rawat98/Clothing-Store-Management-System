using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Index()
        {
            var applicationDbContext = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product);
            return View(applicationDbContext.ToList());
        }

        // GET: OrderItems/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefault(m => m.OrderItemId == id);

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
        public IActionResult Create(OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                _context.SaveChanges();
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
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = _context.OrderItems.Find(id);
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
        public IActionResult Edit(int id, OrderItem orderItem)
        {
            if (id != orderItem.OrderItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    _context.SaveChanges();
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
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var orderItem = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefault(m => m.OrderItemId == id);

            if (orderItem == null) return NotFound();

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}
