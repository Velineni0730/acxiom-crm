using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Lead
    {
        public int LeadId { get; set; }

        public string LeadCode { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string LeadName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [Phone]
        public string Phone { get; set; } = "";

        public string CompanyName { get; set; } = "";
        public string Source { get; set; } = "";
        public string Status { get; set; } = "New";

        [Range(0, double.MaxValue)]
        public decimal ExpectedValue { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string AssignedTo { get; set; } = "";
    }
}