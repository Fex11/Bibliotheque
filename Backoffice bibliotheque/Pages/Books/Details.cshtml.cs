using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Book Book { get; set; } = default!;
        public IList<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Book = await _context.Books
                                 .Include(b => b.Publisher)
                                 .FirstOrDefaultAsync(b => b.Id == id);

            if (Book == null)
                return NotFound();

            // 🔹 Charger les copies du livre
            BookCopies = await _context.BookCopies
                                       .Where(c => c.BookId == Book.Id)
                                       .ToListAsync();

            return Page();
        }
    }
}
