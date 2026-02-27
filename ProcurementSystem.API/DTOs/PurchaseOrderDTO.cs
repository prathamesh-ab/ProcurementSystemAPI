using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.API.DTOs
{
    public class PurchaseOrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }

    public class CreatePurchaseOrderDTO
    {
        [Required]
        public int SupplierId { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDTO> Items { get; set; }
    }

}
