using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();

            ViewBag.OpenLeads = await _context.Leads
                .CountAsync(l =>
                    l.Status != "Converted" &&
                    l.Status != "Lost");

            ViewBag.OpenOpportunities = await _context.Opportunities
                .CountAsync(o =>
                    o.Status != "Lost" &&
                    o.Status != "Won");

            ViewBag.PendingFollowUps = await _context.FollowUps
                .CountAsync(f => f.Status == "Planned");

            ViewBag.Leads = await _context.Leads.CountAsync();
            ViewBag.Customers = await _context.Customers.CountAsync();
            ViewBag.Opportunities = await _context.Opportunities.CountAsync();

            return View();
        }
    }
}
