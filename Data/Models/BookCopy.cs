using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bibliotheque.Models
{
    public class BookCopy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [StringLength(100)]
        public string Barcode { get; set; }

        [StringLength(100)]
        public string ShelfLocation { get; set; }

        public DateTime AcquisitionDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Disponible";

        [StringLength(200)]
        public string BookTitleSnapshot { get; set; }

        [StringLength(200)]
        public string CategoryNamesSnapshot { get; set; }

        [StringLength(200)]
        public string AuthorNamesSnapshot { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        public ICollection<Loan>? Loans { get; set; }

    }
}
