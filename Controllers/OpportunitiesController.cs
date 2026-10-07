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
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;

        public OpportunitiesController(ApplicationDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Opportunities.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            ValidateOpportunity(opportunity);

            if (!ModelState.IsValid)
                return View(opportunity);

            opportunity.CreatedDate = DateTime.Now;
            opportunity.Status = string.IsNullOrWhiteSpace(opportunity.Status)
                ? "Active"
                : opportunity.Status;

            if (User.IsInRole("Sales Executive"))
                opportunity.AssignedTo = User.Identity?.Name ?? "";

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities.FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Opportunity opportunity)
        {
            if (id != opportunity.OpportunityId)
                return NotFound();

            ValidateOpportunity(opportunity);

            if (!ModelState.IsValid)
                return View(opportunity);

            var existing = await _context.Opportunities.FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (existing == null)
                return NotFound();

            existing.OpportunityName = opportunity.OpportunityName;
            existing.CustomerId = opportunity.CustomerId;
            existing.LeadId = opportunity.LeadId;
            existing.Amount = opportunity.Amount;
            existing.Stage = opportunity.Stage;
            existing.Probability = opportunity.Probability;
            existing.ExpectedCloseDate = opportunity.ExpectedCloseDate;
            existing.Status = opportunity.Status;
            existing.AssignedTo = opportunity.AssignedTo;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateOpportunity(Opportunity opportunity)
        {
            if (opportunity.Amount <= 0)
            {
                ModelState.AddModelError(
                    "Amount",
                    "Opportunity Amount must be greater than 0.");
            }

            if (opportunity.Probability < 0 || opportunity.Probability > 100)
            {
                ModelState.AddModelError(
                    "Probability",
                    "Probability must be between 0 and 100.");
            }

            if (opportunity.Status == "Active" &&
                opportunity.ExpectedCloseDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    "ExpectedCloseDate",
                    "Expected Close Date cannot be in the past.");
            }
        }
    }
}