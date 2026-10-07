using AcxiomCRM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        private bool IsSalesExecutive =>
            _httpContextAccessor?.HttpContext?.User
                ?.IsInRole("Sales Executive") == true;

        private string CurrentUserName =>
            _httpContextAccessor?.HttpContext?.User
                ?.Identity?.Name ?? "";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            // Sales Executives can only query records assigned to themselves.
            // Admins and Managers can see the full business scope.
            modelBuilder.Entity<Lead>()
                .HasQueryFilter(l =>
                    !IsSalesExecutive ||
                    l.AssignedTo == CurrentUserName);

            modelBuilder.Entity<Opportunity>()
                .HasQueryFilter(o =>
                    !IsSalesExecutive ||
                    o.AssignedTo == CurrentUserName);

            modelBuilder.Entity<FollowUp>()
                .HasQueryFilter(f =>
                    !IsSalesExecutive ||
                    f.AssignedTo == CurrentUserName);

            modelBuilder.Entity<Activity>()
                .HasQueryFilter(a =>
                    !IsSalesExecutive ||
                    a.AssignedTo == CurrentUserName);
        }
    }
}
