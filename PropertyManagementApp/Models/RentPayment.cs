using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Models
{
    public class RentPayment
    {
        public int Id { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        public DateOnly? PaymentDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public int PropertyId { get; set; }

        public Property? Property { get; set; }

        public int TenantId { get; set; }

        public Tenant? Tenant { get; set; }
    }
}
