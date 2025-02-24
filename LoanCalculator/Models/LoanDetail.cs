namespace LoanCalculator.Models
{
    public class LoanDetail
    {
        public int LoanId { get; set; }
        public int CustomerNo { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTerm { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Insurance { get; set; }
    }
}
