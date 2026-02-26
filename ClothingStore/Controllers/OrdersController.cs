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
        var vm = new OrderVM();
        PopulateOrderCreateDropdowns(vm);

        return View(vm);
    }

    // ================= CREATE (POST) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int customerId, string? status, int[]? productIds, int[]? quantities)
    {
        if (customerId <= 0)
        {
            ModelState.AddModelError("CustomerId", "Please select a customer.");
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            ModelState.AddModelError("Status", "Please select an order status.");
        }

        if (productIds == null || quantities == null || productIds.Length == 0 || productIds.Length != quantities.Length)
        {
            ModelState.AddModelError("", "Please add at least one product with quantity.");
        }

        if (quantities != null && quantities.Any(q => q < 1))
        {
            ModelState.AddModelError("", "Quantity must be at least 1.");
        }

        if (!ModelState.IsValid)
        {
            var vm = new OrderVM
            {
                CustomerId = customerId,
                Status = status ?? "Pending"
            };
            PopulateOrderCreateDropdowns(vm);
            return View(vm);
        }

        var selectedProductIds = productIds!;
        var selectedQuantities = quantities!;
        var orderStatus = status!;

        var pricesByProductId = _context.Products
            .Where(p => selectedProductIds.Contains(p.ProductId))
            .Select(p => new { p.ProductId, p.Price })
            .ToDictionary(p => p.ProductId, p => p.Price);

        if (pricesByProductId.Count != selectedProductIds.Distinct().Count())
        {
            ModelState.AddModelError("", "One or more selected products are invalid.");
            var vm = new OrderVM
            {
                CustomerId = customerId,
                Status = orderStatus
            };
            PopulateOrderCreateDropdowns(vm);
            return View(vm);
        }

        decimal totalAmount = 0m;
        var orderItems = new List<OrderItem>();
        for (int i = 0; i < selectedProductIds.Length; i++)
        {
            if (!pricesByProductId.TryGetValue(selectedProductIds[i], out var price))
            {
                continue;
            }

            totalAmount += price * selectedQuantities[i];
            orderItems.Add(new OrderItem
            {
                ProductId = selectedProductIds[i],
                Quantity = selectedQuantities[i]
            });
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            CustomerId = customerId,
            Status = orderStatus,
            TotalAmount = totalAmount,
            OrderItems = orderItems
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

    // ================= DETAILS =================
    public IActionResult Details(int id)
    {
        var order = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null) return NotFound();
        return View(order);
    }

    // ================= EDIT (POST) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int orderId, int[]? productIds, int[]? quantities, string? status)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null) return NotFound();

        if (productIds == null || quantities == null || productIds.Length == 0 || productIds.Length != quantities.Length)
        {
            ModelState.AddModelError("", "Please add at least one product with quantity.");
        }
        if (quantities != null && quantities.Any(q => q < 1))
        {
            ModelState.AddModelError("", "Quantity must be at least 1.");
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            ModelState.AddModelError("", "Order status is required.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name })
                .ToList();
            ViewBag.OrderStatus = order.Status;

            return View(order);
        }

        var selectedProductIds = productIds!;
        var selectedQuantities = quantities!;
        var orderStatus = status!;

        // Remove old items
        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.SaveChanges();

        // Add updated items
        for (int i = 0; i < selectedProductIds.Length; i++)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = selectedProductIds[i],
                Quantity = selectedQuantities[i]
            });
        }

        var pricesByProductId = _context.Products
            .Where(p => selectedProductIds.Contains(p.ProductId))
            .Select(p => new { p.ProductId, p.Price })
            .ToDictionary(p => p.ProductId, p => p.Price);

        decimal totalAmount = 0m;
        for (int i = 0; i < selectedProductIds.Length; i++)
        {
            if (pricesByProductId.TryGetValue(selectedProductIds[i], out var price))
            {
                totalAmount += price * selectedQuantities[i];
            }
        }

        order.Status = orderStatus;
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

    private void PopulateOrderCreateDropdowns(OrderVM vm)
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
    }
}
