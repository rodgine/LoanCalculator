using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanAPI.Models
{
    [Table("CustomerDtl")]
    public class CustomerDtl
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [ForeignKey("CustomerTbl")]
        public int? CustomerNo { get; set; }

        public int EquityTerm { get; set; }
        public decimal EquityAmount { get; set; }
        public int MATerm { get; set; }
        public decimal MAAmount { get; set; }
        public int MIR { get; set; }
        public int FIRE { get; set; }
        public decimal LoanAmt { get; set; }
        public decimal IntRate { get; set; }

        [JsonIgnore] // Ignore this in API requests
        public CustomerTbl? Customer { get; set; }
    }
}
