using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Loan> Loans { get; set; } = new();

        // Filtres
        [BindProperty(SupportsGet = true)]
        public string? UserName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BookTitle { get; set; }

        // Status déjà existant
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        // SelectList pour le status
        public SelectList Statuses { get; set; } = new SelectList(new[]
        {
            new { Value = "create", Text = "Create" },
            new { Value = "onloan", Text = "On Loan" },
            new { Value = "returned", Text = "Returned" },
            new { Value = "late", Text = "Late" }
        }, "Value", "Text");

        public int PageSize { get; set; } = 2; // nombre de lignes par page
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1; // page courante
        public int TotalPages { get; set; }


        public async Task OnGetAsync()
        {
            IQueryable<Loan> query = _context.Loans
                                             .Include(l => l.BookCopy)
                                             .Include(l => l.Borrower);

            // Filtre par user
            if (!string.IsNullOrEmpty(UserName))
                query = query.Where(l => (l.Borrower.FirstName + " " + l.Borrower.LastName).Contains(UserName));

            // Filtre par book
            if (!string.IsNullOrEmpty(BookTitle))
                query = query.Where(l => l.BookCopy.BookTitleSnapshot.Contains(BookTitle));

            // Filtre par status
            if (!string.IsNullOrEmpty(Status))
                query = query.Where(l => l.Status == Status);

            // compter total
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // récupérer la page courante
            Loans = await query
                .OrderByDescending(l => l.LoanDate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }


        // Valider un emprunt (create → onloan)
        // Valider un emprunt (create → onloan)
        public async Task<IActionResult> OnPostValidateAsync(int id)
        {
            var loan = await _context.Loans
             .Include(l => l.BookCopy)
             .Include(l => l.Book)
             .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound();

            loan.Status = "onLoan";
            loan.LoanDate = DateTime.Now;
            loan.DueDate = DateTime.Now.AddDays(14);

            // Rendre le BookCopy indisponible
            if (loan.BookCopy != null)
            {
                loan.BookCopy.Status = "Indisponible"; // Assure-toi d'avoir une propriété IsAvailable
            }

            if (loan.Book != null)
            {
                loan.Book.AvailableCopiesCount -= 1; // Assure-toi d'avoir une propriété IsAvailable
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }


        // Retourner un livre (onloan → returned)
        public async Task<IActionResult> OnPostReturnAsync(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.BookCopy)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound();

            // 1️⃣ Mise à jour du prêt
            loan.Status = "returned";
            loan.ReturnDate = DateTime.Now;

            if (loan.BookCopy == null || loan.Book == null)
                return BadRequest();

            // 2️⃣ Chercher la prochaine réservation en attente
            var nextReservation = await _context.Reservations
                .Where(r => r.BookId == loan.BookId && r.Status == "pending")
                .OrderBy(r => r.PositionInQueue)
                .FirstOrDefaultAsync();

            if (nextReservation != null)
            {
                // 3️⃣ Il y a une réservation → réserver le copy
                loan.BookCopy.Status = "Reserved";

                nextReservation.Status = "approved";
                nextReservation.ExpireAt = DateTime.Now.AddDays(2); // délai de retrait

                // ⚠️ Le livre reste avec le même nombre de copies disponibles
            }
            else
            {
                // 4️⃣ Aucune réservation → copy disponible
                loan.BookCopy.Status = "Disponible";
                loan.Book.AvailableCopiesCount += 1;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLateAsync(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.BookCopy)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound();

            // 1️⃣ Mise à jour du prêt
            loan.Status = "late";

            if (loan.BookCopy == null || loan.Book == null)
                return BadRequest();

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

    }
}
