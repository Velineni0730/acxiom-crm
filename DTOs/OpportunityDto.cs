namespace AcxiomCRM.DTOs
{
    public class OpportunityDto
    {
        public int OpportunityId { get; set; }
        public string OpportunityName { get; set; } = "";
        public int? CustomerId { get; set; }
        public int? LeadId { get; set; }
        public decimal Amount { get; set; }
        public string Stage { get; set; } = "";
        public decimal Probability { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string AssignedTo { get; set; } = "";
    }
}
