using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalLeads = await _context.Leads.CountAsync();
            ViewBag.ConvertedLeads = await _context.Leads
                .CountAsync(l => l.Status == "Converted");

            ViewBag.TotalOpportunities = await _context.Opportunities.CountAsync();

            ViewBag.WonOpportunities = await _context.Opportunities
                .CountAsync(o => o.Stage == "Won");

            ViewBag.TotalPipeline = await _context.Opportunities
                .Where(o => o.Status == "Active")
                .SumAsync(o => o.Amount);

            ViewBag.WeightedPipeline = await _context.Opportunities
                .Where(o => o.Status == "Active")
                .SumAsync(o => o.Amount * o.Probability / 100);

            ViewBag.PendingFollowUps = await _context.FollowUps
                .CountAsync(f => f.Status == "Planned");

            return View();
        }
    }
}
