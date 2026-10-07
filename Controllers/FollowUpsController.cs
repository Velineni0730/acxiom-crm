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
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;

        public FollowUpsController(ApplicationDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var followUps = await _context.FollowUps
                .OrderBy(f => f.FollowUpDate)
                .ToListAsync();

            return View(followUps);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            if (followUp.FollowUpDate.Date < DateTime.Today &&
                (followUp.Status == "Planned" || followUp.Status == ""))
            {
                ModelState.AddModelError(
                    "FollowUpDate",
                    "Follow-up date cannot be earlier than today.");
            }

            if (!ModelState.IsValid)
                return View(followUp);

            if (string.IsNullOrWhiteSpace(followUp.Status))
                followUp.Status = "Planned";

            if (User.IsInRole("Sales Executive"))
                followUp.AssignedTo = User.Identity?.Name ?? "";

            _context.FollowUps.Add(followUp);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "Create", "FollowUp",
                followUp.FollowUpId.ToString(),
                null, followUp.Remarks);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var followUp = await _context.FollowUps.FirstOrDefaultAsync(f => f.FollowUpId == id);

            if (followUp == null)
                return NotFound();

            followUp.Status = "Completed";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var followUp = await _context.FollowUps.FirstOrDefaultAsync(f => f.FollowUpId == id);

            if (followUp == null)
                return NotFound();

            followUp.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}