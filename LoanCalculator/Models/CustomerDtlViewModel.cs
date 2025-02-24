using System.ComponentModel.DataAnnotations.Schema;

namespace LoanCalculator.Models
{
    public class CustomerDtlViewModel
    {
        public int? CustomerNo { get; set; }  // Add CustomerNo here
        public int EquityTerm { get; set; }
        public decimal EquityAmount { get; set; }
        public int MATerm { get; set; }
        public decimal MAAmount { get; set; }
        public int MIR { get; set; }
        public int FIRE { get; set; }
        public decimal LoanAmt { get; set; }
        public decimal IntRate { get; set; }
    }
}
