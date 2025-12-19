using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bibliotheque.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string NormalizedTitle { get; set; } = "";
        public string Subtitle { get; set; }

        [Display(Name = "Publication Year")]
        public int PublicationYear { get; set; }

        // Relation avec Publisher
        [Display(Name = "Publisher")]
        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }


        [Display(Name = "Cover Image URL")]
        public string CoverImageUrl { get; set; } = "";

        [Display(Name = "Authors")]
        public string AuthorNamesText { get; set; } = ""; // texte combiné pour affichage rapide

        [Display(Name = "Categories")]
        public string CategoryNamesText { get; set; } = "";// texte combiné pour affichage rapide

        public string Keyword { get; set; }

        [Display(Name = "Total Copies")]
        public int TotalCopiesCount { get; set; } = 0;

        [Display(Name = "Available Copies")]
        public int AvailableCopiesCount { get; set; } = 0;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Relations Many-to-Many
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<Loan>? Loans { get; set; }
    }

}
