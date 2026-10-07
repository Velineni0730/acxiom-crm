using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;

        public CustomersController(
            ApplicationDbContext context,
            AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var customers = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchString) ||
                    c.Email.Contains(searchString) ||
                    c.Phone.Contains(searchString) ||
                    c.CompanyName.Contains(searchString));
            }

            return View(await customers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "A customer with this email already exists.");
            }

            if (await _context.Customers.AnyAsync(c => c.Phone == customer.Phone))
            {
                ModelState.AddModelError(
                    "Phone",
                    "A customer with this phone number already exists.");
            }

            if (!ModelState.IsValid)
                return View(customer);

            customer.CustomerCode =
                "CUS-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            customer.CreatedDate = DateTime.Now;
            customer.CreatedBy = User.Identity?.Name ?? "System";

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "Create",
                "Customer",
                customer.CustomerId.ToString(),
                null,
                customer.CustomerName);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
                return NotFound();

            if (await _context.Customers.AnyAsync(c =>
                c.Email == customer.Email &&
                c.CustomerId != id))
            {
                ModelState.AddModelError(
                    "Email",
                    "A customer with this email already exists.");
            }

            if (await _context.Customers.AnyAsync(c =>
                c.Phone == customer.Phone &&
                c.CustomerId != id))
            {
                ModelState.AddModelError(
                    "Phone",
                    "A customer with this phone number already exists.");
            }

            if (!ModelState.IsValid)
                return View(customer);

            var existingCustomer =
                await _context.Customers.FindAsync(id);

            if (existingCustomer == null)
                return NotFound();

            var oldName = existingCustomer.CustomerName;

            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.Address = customer.Address;
            existingCustomer.City = customer.City;
            existingCustomer.State = customer.State;
            existingCustomer.Status = customer.Status;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "Update",
                "Customer",
                existingCustomer.CustomerId.ToString(),
                oldName,
                existingCustomer.CustomerName);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                var customerName = customer.CustomerName;
                var customerId = customer.CustomerId;

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    "Delete",
                    "Customer",
                    customerId.ToString(),
                    customerName,
                    null);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
