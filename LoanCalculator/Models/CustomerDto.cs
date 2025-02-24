namespace LoanCalculator.Models
{
    public class CustomerDto
    {
        public int CustomerNo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string UnitID { get; set; }
        public int EquityTerm { get; set; }
        public decimal EquityAmount { get; set; }
        public int MATerm { get; set; }
        public decimal MAAmount { get; set; }
        public int MIR { get; set; }
        public int FIRE { get; set; }
        public decimal LoanAmt { get; set; }
        public decimal IntRate { get; set; }
        public string Type { get; set; }
        public decimal HousePrice { get; set; }
        public decimal LotPrice { get; set; }
    }
}
