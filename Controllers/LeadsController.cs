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
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;

        public LeadsController(ApplicationDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var leads = _context.Leads.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                leads = leads.Where(l =>
                    l.LeadName.Contains(searchString) ||
                    l.Email.Contains(searchString) ||
                    l.Phone.Contains(searchString) ||
                    l.CompanyName.Contains(searchString));
            }

            return View(await leads.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.LeadId == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead lead)
        {
            if (!ModelState.IsValid)
                return View(lead);

            lead.LeadCode = "LEAD-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            lead.CreatedDate = DateTime.Now;
            lead.Status = string.IsNullOrWhiteSpace(lead.Status)
                ? "New"
                : lead.Status;

            if (User.IsInRole("Sales Executive"))
                lead.AssignedTo = User.Identity?.Name ?? "";

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lead lead)
        {
            if (id != lead.LeadId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(lead);

            var existingLead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id);

            if (existingLead == null)
                return NotFound();

            existingLead.LeadName = lead.LeadName;
            existingLead.Email = lead.Email;
            existingLead.Phone = lead.Phone;
            existingLead.CompanyName = lead.CompanyName;
            existingLead.Source = lead.Source;
            existingLead.Status = lead.Status;
            existingLead.ExpectedValue = lead.ExpectedValue;
            existingLead.AssignedTo = lead.AssignedTo;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.LeadId == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id);

            if (lead != null)
            {
                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convert(int id)
        {
            var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id);

            if (lead == null)
                return NotFound();

            if (lead.Status != "Qualified")
            {
                TempData["Error"] = "Only qualified leads can be converted.";
                return RedirectToAction(nameof(Index));
            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Email == lead.Email || c.Phone == lead.Phone);
            
            if (customerExists)
            {
                TempData["Error"] =
                    "A customer with the same email or phone number already exists.";
            
                return RedirectToAction(nameof(Index));
            }

            var customer = new Customer
            {
                CustomerCode = "CUS-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CustomerName = lead.LeadName,
                Email = lead.Email,
                Phone = lead.Phone,
                CompanyName = lead.CompanyName,
                Status = "Active",
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Customers.Add(customer);

            lead.Status = "Converted";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Lead converted to customer successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}