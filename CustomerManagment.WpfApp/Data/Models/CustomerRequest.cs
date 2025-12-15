using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagment.WpfApp.Data.Models
{
    public class CustomerRequest
    {
        public int Id { get; set; }

        [Required]
        public required string CustomerRequestDescription { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // Add foreign key to Customer
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }
    }
}