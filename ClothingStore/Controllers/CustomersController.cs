using ClothingStore.Data;
using ClothingStore.Models;
using ClothingStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var customer = new Customer
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Phone = vm.Phone
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Customer added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Edit/5
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            var vm = new CustomerVM
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone
            };
            return View(vm);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CustomerVM vm)
        {
            if (id != vm.CustomerId) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            customer.FirstName = vm.FirstName;
            customer.LastName = vm.LastName;
            customer.Email = vm.Email;
            customer.Phone = vm.Phone;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Customer updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Customer deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}