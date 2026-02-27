using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.API.DTOs
{
    public class SupplierDTO
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Country { get; set; }
        public decimal? Rating { get; set; }
    }

    public class CreateSupplierDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string SupplierName { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Country { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }
    }

    public class UpdateSupplierDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string SupplierName { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }
    }
}
