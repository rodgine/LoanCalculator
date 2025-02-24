using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanAPI.Models
{
    [Table("CustomerTbl")]
    public class CustomerTbl
    {
        [Key]
        public int CustomerNo { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [ForeignKey("InventoryTbl")]
        public string UnitID { get; set; }

        [JsonIgnore] // Ignore this in API requests
        public InventoryTbl? Inventory { get; set; }

        public ICollection<CustomerDtl>? CustomerDtl { get; set; } = new List<CustomerDtl>();
    }
}
