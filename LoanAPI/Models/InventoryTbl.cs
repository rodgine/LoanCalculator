using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanAPI.Models
{
    [Table("InventoryTbl")]
    public class InventoryTbl
    {
        [Key, MaxLength(20)]
        public string UnitID { get; set; }

        [Required, MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public decimal HousePrice { get; set; }

        [Required]
        public decimal LotPrice { get; set; }

        [JsonIgnore] // Ignore this in API requests
        public ICollection<CustomerTbl> Customers { get; set; }
    }
}
