namespace ProcurementSystem.API.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Country { get; set; }
        public decimal? Rating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}