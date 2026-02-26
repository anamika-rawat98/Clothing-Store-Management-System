using ClothingStore.Data;
using ClothingStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClothingStore.ViewModels;
using System.Linq;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= INDEX =================
    public IActionResult Index()
    {
        var orders = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToList();

        return View(orders);
    }

    // ================= CREATE (GET) =================
    public IActionResult Create()
    {
        var vm = new OrderVM
        {
            Customers = _context.Customers.Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.FirstName + " " + c.LastName
            }).ToList(),

            Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList()
        };

        return View(vm);
    }

    // ================= CREATE (POST) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(OrderVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Customers = _context.Customers.Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.FirstName + " " + c.LastName
            }).ToList();

            vm.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

            return View(vm);
        }

        var selectedProduct = _context.Products.FirstOrDefault(p => p.ProductId == vm.ProductId);
        if (selectedProduct == null)
        {
            ModelState.AddModelError(nameof(vm.ProductId), "Selected product is invalid.");

            vm.Customers = _context.Customers.Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.FirstName + " " + c.LastName
            }).ToList();

            vm.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

            return View(vm);
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            CustomerId = vm.CustomerId,
            Status = vm.Status,
            TotalAmount = selectedProduct.Price * vm.Quantity,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = vm.ProductId,
                    Quantity = vm.Quantity
                }
            }
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Order created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // ================= EDIT (GET) =================
    public IActionResult Edit(int id)
    {
        var order = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null) return NotFound();

        // Products dropdown for existing items
        ViewBag.Products = _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            })
            .ToList();
        ViewBag.OrderStatus = order.Status;

        return View(order);
    }

    // ================= EDIT (POST) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int orderId, int[] productIds, int[] quantities, string status)
    {
        if (productIds.Length != quantities.Length)
        {
            ModelState.AddModelError("", "Products and quantities mismatch.");
        }

        var order = _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name })
                .ToList();
            ViewBag.OrderStatus = order.Status;

            return View(order);
        }

        // Remove old items
        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.SaveChanges();

        // Add updated items
        for (int i = 0; i < productIds.Length; i++)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = productIds[i],
                Quantity = quantities[i]
            });
        }

        var pricesByProductId = _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => new { p.ProductId, p.Price })
            .ToDictionary(p => p.ProductId, p => p.Price);

        decimal totalAmount = 0m;
        for (int i = 0; i < productIds.Length; i++)
        {
            if (pricesByProductId.TryGetValue(productIds[i], out var price))
            {
                totalAmount += price * quantities[i];
            }
        }

        order.Status = string.IsNullOrWhiteSpace(status) ? order.Status : status;
        order.TotalAmount = totalAmount;

        _context.SaveChanges();

        TempData["SuccessMessage"] = "Order updated successfully!";
        return RedirectToAction(nameof(Edit), new { id = orderId }); // Stay on the same page
    }

    // ================= DELETE =================
    public IActionResult Delete(int id)
    {
        var order = _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var order = _context.Orders.Find(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        TempData["SuccessMessage"] = "Order deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
}
