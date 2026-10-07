using AcxiomCRM.Data;
using AcxiomCRM.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api
{
    [ApiController]
    [Route("api/leads")]
    [Authorize]
    public class LeadsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadDto>>> GetLeads()
        {
            var leads = await _context.Leads
                .AsNoTracking()
                .Select(l => new LeadDto
                {
                    LeadId = l.LeadId,
                    LeadCode = l.LeadCode,
                    LeadName = l.LeadName,
                    Email = l.Email,
                    Phone = l.Phone,
                    CompanyName = l.CompanyName,
                    Source = l.Source,
                    Status = l.Status,
                    ExpectedValue = l.ExpectedValue,
                    CreatedDate = l.CreatedDate,
                    AssignedTo = l.AssignedTo
                })
                .ToListAsync();

            return Ok(leads);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeadDto>> GetLead(int id)
        {
            var lead = await _context.Leads
                .AsNoTracking()
                .Where(l => l.LeadId == id)
                .Select(l => new LeadDto
                {
                    LeadId = l.LeadId,
                    LeadCode = l.LeadCode,
                    LeadName = l.LeadName,
                    Email = l.Email,
                    Phone = l.Phone,
                    CompanyName = l.CompanyName,
                    Source = l.Source,
                    Status = l.Status,
                    ExpectedValue = l.ExpectedValue,
                    CreatedDate = l.CreatedDate,
                    AssignedTo = l.AssignedTo
                })
                .FirstOrDefaultAsync();

            if (lead == null)
                return NotFound();

            return Ok(lead);
        }
    }
}
