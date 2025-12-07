using System.ComponentModel.DataAnnotations;
using MiniTweeterBackend.Helpers.Constants;

namespace MiniTweeterBackend.Models
{

    public class Payments
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public string Status { get; set; } = PaymentStatus.Pending;
        public Dictionary<string, object> ProviderResponse { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
