using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanCalculator.API.Models
{
    public class LoanDetail
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public int CustomerNo { get; set; } 

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public int LoanTerm { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal Principal { get; set; }

        [Required]
        public decimal Interest { get; set; }

        [Required]
        public decimal Insurance { get; set; }
    }
}
