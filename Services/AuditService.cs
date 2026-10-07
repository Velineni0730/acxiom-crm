using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AcxiomCRM.Services
{
    public class AuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string recordId,
            string? oldValue = null,
            string? newValue = null)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var audit = new AuditLog
            {
                UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                Action = action,
                EntityName = entityName,
                RecordId = recordId,
                OldValue = oldValue ?? "",
                NewValue = newValue ?? "",
                CreatedDate = DateTime.UtcNow,
                IpAddress = _httpContextAccessor.HttpContext?
                    .Connection.RemoteIpAddress?.ToString() ?? ""
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}
