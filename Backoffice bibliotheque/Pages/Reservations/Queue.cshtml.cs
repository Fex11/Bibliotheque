using Bibliotheque.Data;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backoffice_bibliotheque.Pages.Reservations
{
    public class QueueModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public QueueModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Book Book { get; set; }
        public List<Reservation> Reservations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int bookId)
        {
            Book = await _context.Books.FindAsync(bookId);
            if (Book == null) return NotFound();

            Reservations = await _context.Reservations
                .Include(r => r.Requester)
                .Where(r => r.BookId == bookId && (r.Status == "pending" || r.Status == "approved"))
                .OrderBy(r => r.PositionInQueue)
                .ToListAsync();

            return Page();
        }

        // Approuver
        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();

            reservation.Status = "approved";
            reservation.ExpireAt = DateTime.Now.AddDays(2);

            // Décaler la file
            var others = await _context.Reservations
                .Where(r => r.BookId == reservation.BookId
                         && r.Status == "pending"
                         && r.PositionInQueue > reservation.PositionInQueue)
                .ToListAsync();

            foreach (var r in others)
                r.PositionInQueue--;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { bookId = reservation.BookId });
        }

        // Annuler
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            // Annuler la réservation
            reservation.Status = "cancelled";

            // Décrémenter la position des suivantes
            var others = await _context.Reservations
                .Where(r => r.BookId == reservation.BookId
                         && r.Status == "pending"
                         && r.PositionInQueue > reservation.PositionInQueue)
                .ToListAsync();

            foreach (var r in others)
                r.PositionInQueue--;

            // Valider la nouvelle première réservation si elle existe
            var firstPending = await _context.Reservations
                .Where(r => r.BookId == reservation.BookId && r.Status == "pending")
                .OrderBy(r => r.PositionInQueue)
                .FirstOrDefaultAsync();

            if (firstPending != null)
            {
                firstPending.Status = "approved";
            }
            else
            {
                var copyToFree = await _context.BookCopies
                .Where(c => c.BookId == reservation.BookId && c.Status == "Reserved")
                .OrderBy(c => c.Id) // peu importe, on en prend une
                .FirstOrDefaultAsync();

                if (copyToFree != null)
                {
                    copyToFree.Status = "Disponible";
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { bookId = reservation.BookId });
        }
    }

}
