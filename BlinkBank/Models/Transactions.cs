using System.ComponentModel.DataAnnotations;

namespace BlinkBank.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Transaction type is required.")]
        [StringLength(50, ErrorMessage = "Transaction type cannot exceed 50 characters.")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(20, ErrorMessage = "Source account number cannot exceed 20 characters.")]
        public string? sourceAccountNumber { get; set; }

        [StringLength(20, ErrorMessage = "Target account number cannot exceed 20 characters.")]
        public string? targetAccountNumber { get; set; }

        public DateTime Date { get; set; }

        public Accounts Account { get; set; }

    }
}
