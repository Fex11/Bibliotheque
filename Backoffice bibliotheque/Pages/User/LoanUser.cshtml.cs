using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Backoffice_bibliotheque.Pages.User
{
    public class LoanUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoanUserModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public LibraryUser User { get; set; } = default!;
        public List<Loan> Loans { get; set; } = new List<Loan>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            User = await _context.LibraryUsers.FindAsync(id);
            if (User == null) return NotFound();

            // Inclure BookCopy et éventuellement Book si nécessaire
            Loans = await _context.Loans
                                  .Include(l => l.BookCopy)
                                  .ThenInclude(bc => bc.Book) // si tu veux accès au titre original
                                  .Where(l => l.BorrowerId == id)
                                  .OrderByDescending(l => l.LoanDate)
                                  .ToListAsync();

            return Page();
        }
    }
}
