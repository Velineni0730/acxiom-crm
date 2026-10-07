namespace AcxiomCRM.DTOs
{
    public class LeadDto
    {
        public int LeadId { get; set; }
        public string LeadCode { get; set; } = "";
        public string LeadName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Source { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal ExpectedValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AssignedTo { get; set; } = "";
    }
}
