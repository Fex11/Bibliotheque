using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backoffice_bibliotheque.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        /* =========================
         * Relations
         * ========================= */

        [Required]
        public int BookCopyId { get; set; }

        [ForeignKey(nameof(BookCopyId))]
        public BookCopy? BookCopy { get; set; }

        [Required]
        public int BorrowerId { get; set; }

        [ForeignKey(nameof(BorrowerId))]
        public LibraryUser? Borrower { get; set; }

        /* =========================
         * Données métier
         * ========================= */

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "OnLoan";
        // OnLoan | Returned | Late | Lost

        public int RenewalCount { get; set; } = 0;

        public DateTime? LastReminderDate { get; set; }

        /* =========================
         * Snapshots (historique)
         * ========================= */

        [Required]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [MaxLength(255)]
        public string BookTitleSnapshot { get; set; } = "";

        [MaxLength(150)]
        public string BorrowerNameSnapshot { get; set; } = "";

        [MaxLength(150)]
        public string BorrowerEmailSnapshot { get; set; } = "";

        /* =========================
         * Audit
         * ========================= */

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
