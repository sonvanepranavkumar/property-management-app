using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Models
{
    public class Tenant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public DateOnly LeaseStartDate { get; set; }

        public DateOnly? LeaseEndDate { get; set; }

        public int PropertyId { get; set; }

        public Property? Property { get; set; }

        public ICollection<RentPayment> RentPayments { get; set; } = new List<RentPayment>();
    }
}
