using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagment.WpfApp.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public required string CustomerName { get; set; }

        public string? CustomerJobTitle { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerMobileNumber { get; set; }

        public string? CustomerPhoneNumber { get; set; }

        
        public string? CustomerEmail { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public int? CustomerRequest_Id { get; set; }

        public virtual ICollection<CustomerRequest>? CustomerRequests { get; set; }
    }
}