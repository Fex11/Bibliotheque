using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backoffice_bibliotheque.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        // Le livre demandé
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        // L'utilisateur qui demande
        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public LibraryUser? Requester { get; set; }

        // Statut : pending, assigned, cancelled, expired
        [StringLength(20)]
        public string Status { get; set; } = "pending";

        // Position dans la file d'attente
        public int PositionInQueue { get; set; } = 1;

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public DateTime? ExpireAt { get; set; }

        // Snapshots pour ne pas dépendre des mises à jour de Book ou User
        [StringLength(200)]
        public string BookTitleSnapshot { get; set; } = string.Empty;

        [StringLength(200)]
        public string RequesterNameSnapshot { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
