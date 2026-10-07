using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditService _auditService;

        public ActivitiesController(
            ApplicationDbContext context,
            AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Activities
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Activity activity)
        {
            if (!ModelState.IsValid)
                return View(activity);

            if (string.IsNullOrWhiteSpace(activity.Status))
                activity.Status = "Planned";

            activity.ActivityDate = activity.ActivityDate == default
                ? DateTime.Now
                : activity.ActivityDate;

            if (User.IsInRole("Sales Executive"))
                activity.AssignedTo = User.Identity?.Name ?? "";

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "Create",
                "Activity",
                activity.ActivityId.ToString(),
                null,
                activity.Subject);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == id);

            if (activity == null)
                return NotFound();

            activity.Status = "Completed";
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "Complete",
                "Activity",
                activity.ActivityId.ToString(),
                "Planned",
                "Completed");

            return RedirectToAction(nameof(Index));
        }
    }
}
