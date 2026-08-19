using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Rent { get; set; }

        public string Status => Tenants.Any() ? "Occupied" : "Vacant";

        public string LandlordId { get; set; } = string.Empty;

        public PropertyManagementAppUser? Landlord { get; set; }

        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

        public ICollection<RentPayment> RentPayments { get; set; } = new List<RentPayment>();

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
