namespace LoanCalculator.Models
{
    public class CustomerViewModel
    {
        public int CustomerNo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string UnitID { get; set; }
        public CustomerDtlViewModel CustomerDtl { get; set; }
        public InventoryViewModel Inventory { get; set; }
    }
}
