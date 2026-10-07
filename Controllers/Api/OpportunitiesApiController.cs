using AcxiomCRM.Data;
using AcxiomCRM.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api
{
    [ApiController]
    [Route("api/opportunities")]
    [Authorize]
    public class OpportunitiesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OpportunitiesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OpportunityDto>>> GetOpportunities()
        {
            var opportunities = await _context.Opportunities
                .AsNoTracking()
                .Select(o => new OpportunityDto
                {
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,
                    CustomerId = o.CustomerId,
                    LeadId = o.LeadId,
                    Amount = o.Amount,
                    Stage = o.Stage,
                    Probability = o.Probability,
                    ExpectedCloseDate = o.ExpectedCloseDate,
                    Status = o.Status,
                    CreatedDate = o.CreatedDate,
                    AssignedTo = o.AssignedTo
                })
                .ToListAsync();

            return Ok(opportunities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OpportunityDto>> GetOpportunity(int id)
        {
            var opportunity = await _context.Opportunities
                .AsNoTracking()
                .Where(o => o.OpportunityId == id)
                .Select(o => new OpportunityDto
                {
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,
                    CustomerId = o.CustomerId,
                    LeadId = o.LeadId,
                    Amount = o.Amount,
                    Stage = o.Stage,
                    Probability = o.Probability,
                    ExpectedCloseDate = o.ExpectedCloseDate,
                    Status = o.Status,
                    CreatedDate = o.CreatedDate,
                    AssignedTo = o.AssignedTo
                })
                .FirstOrDefaultAsync();

            if (opportunity == null)
                return NotFound();

            return Ok(opportunity);
        }
    }
}
