namespace LoanCalculator.Models
{
    public class CustomerDtlDto
    {
        public int CustomerNo { get; set; }  // From CustomerNumber
        public int EquityTerm { get; set; }  // Derived (LoanTerm / 2)
        public decimal EquityAmount { get; set; } // Loan * 0.20
        public int MATerm { get; set; } // LoanTerm
        public decimal MAAmount { get; set; } // LoanAmt / LoanTerm
        public decimal MIR { get; set; } // Default 1% (or business rule)
        public decimal FIRE { get; set; } // LoanAmt * 0.0025
        public decimal LoanAmt { get; set; } // From Loan
        public decimal IntRate { get; set; } // MIR * LoanTerm

        // Constructor to auto-calculate fields
        public CustomerDtlDto(decimal loan, int loanTerm, int customerNo)
        {
            CustomerNo = customerNo;
            EquityTerm = loanTerm / 2;
            EquityAmount = loan * 0.20m;
            MATerm = loanTerm;
            MAAmount = loan / loanTerm;
            MIR = 0.01m;
            FIRE = loan * 0.0025m;
            LoanAmt = loan;
            IntRate = MIR * loanTerm;
        }
    }
}
