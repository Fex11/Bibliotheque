using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Loans
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Loan Loan { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? ReservationId { get; set; }

        public SelectList Users { get; set; } = default!;
        public SelectList Books { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await LoadListsAsync();

            if (ReservationId.HasValue)
            {
                await LoadFromReservationAsync(ReservationId.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            var copy = await _context.BookCopies
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.Id == Loan.BookCopyId);

            var user = await _context.LibraryUsers.FindAsync(Loan.BorrowerId);

            if (copy == null || user == null)
            {
                ModelState.AddModelError("", "Invalid data");
                await LoadListsAsync();
                return Page();
            }

            // SNAPSHOTS
            Loan.BookId = copy.BookId;
            Loan.BookTitleSnapshot = copy.Book!.Title;
            Loan.BorrowerNameSnapshot = $"{user.FirstName} {user.LastName}";
            Loan.BorrowerEmailSnapshot = user.Email;

            Loan.Status = "created";
            Loan.LoanDate = DateTime.Now;
            Loan.DueDate = DateTime.Now.AddDays(14);

            // Si emprunt issu d'une réservation
            if (ReservationId.HasValue)
            {
                var reservation = await _context.Reservations.FindAsync(ReservationId.Value);
                if (reservation != null)
                {
                    reservation.Status = "confirmed";

                    // Réorganiser la queue
                    var others = await _context.Reservations
                        .Where(r => r.BookId == reservation.BookId
                                 && (r.Status == "pending" || r.Status == "approved")
                                 && r.PositionInQueue > reservation.PositionInQueue)
                        .OrderBy(r => r.PositionInQueue)
                        .ToListAsync();

                    foreach (var r in others)
                    {
                        r.PositionInQueue--;
                    }
                    reservation.PositionInQueue=0;
                }
            }

            _context.Loans.Add(Loan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // AJAX copies
        public async Task<JsonResult> OnGetCopiesAsync(int bookId, int? reservationId)
        {
            var statusFilter = reservationId.HasValue ? "Reserved" : "Disponible";

            var copies = await _context.BookCopies
            .Where(c => c.BookId == bookId && c.Status == statusFilter)
            .Select(c => new
            {
                c.Id,
                label = $"{c.Barcode} - {c.ShelfLocation}"
            })
            .ToListAsync();

            return new JsonResult(copies);
            
        }

        private async Task LoadListsAsync()
        {
            Users = new SelectList(
                await _context.LibraryUsers
                    .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName })
                    .ToListAsync(),
                "Id", "Name");

            Books = new SelectList(
                await _context.Books.ToListAsync(),
                "Id", "Title");
        }

        private async Task LoadFromReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Requester)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null) return;

            Loan.BookId = reservation.BookId;
            Loan.BorrowerId = reservation.RequesterId;
        }
    }
}
