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

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var from = fromDate?.Date;
            var to = toDate?.Date.AddDays(1);

            var customers = _context.Customers.AsQueryable();
            var leads = _context.Leads.AsQueryable();
            var opportunities = _context.Opportunities.AsQueryable();
            var followUps = _context.FollowUps.AsQueryable();

            if (from.HasValue)
            {
                customers = customers.Where(c => c.CreatedDate >= from.Value);
                leads = leads.Where(l => l.CreatedDate >= from.Value);
                opportunities = opportunities.Where(o => o.CreatedDate >= from.Value);
                followUps = followUps.Where(f => f.FollowUpDate >= from.Value);
            }

            if (to.HasValue)
            {
                customers = customers.Where(c => c.CreatedDate < to.Value);
                leads = leads.Where(l => l.CreatedDate < to.Value);
                opportunities = opportunities.Where(o => o.CreatedDate < to.Value);
                followUps = followUps.Where(f => f.FollowUpDate < to.Value);
            }

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            ViewBag.TotalCustomers = await customers.CountAsync();

            ViewBag.OpenLeads = await leads.CountAsync(l =>
                l.Status != "Converted" &&
                l.Status != "Lost" &&
                l.Status != "Unqualified");

            ViewBag.OpenOpportunities = await opportunities.CountAsync(o =>
                o.Status == "Active");

            ViewBag.PendingFollowUps = await followUps.CountAsync(f =>
                f.Status == "Planned");

            ViewBag.PipelineValue = await opportunities
                .Where(o => o.Status == "Active")
                .SumAsync(o => o.Amount);

            var pipeline = await opportunities
                .Where(o => o.Status == "Active")
                .GroupBy(o => o.Stage)
                .Select(g => new
                {
                    Stage = g.Key,
                    Amount = g.Sum(o => o.Amount)
                })
                .ToListAsync();

            var leadStatus = await leads
                .GroupBy(l => l.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.PipelineLabels = pipeline.Select(x => x.Stage).ToList();
            ViewBag.PipelineValues = pipeline.Select(x => x.Amount).ToList();

            ViewBag.LeadStatusLabels = leadStatus.Select(x => x.Status).ToList();
            ViewBag.LeadStatusValues = leadStatus.Select(x => x.Count).ToList();

            return View();
        }
    }
}
