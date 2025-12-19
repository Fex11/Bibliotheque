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
    public class DeleteModel : PageModel
    {
        private readonly Backoffice_bibliotheque.Data.ApplicationDbContext _context;

        public DeleteModel(Backoffice_bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            // Inclure Publisher pour avoir le nom
            var book = await _context.Books
                                     .Include(b => b.Publisher)
                                     .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            Book = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                Book = book;
                _context.Books.Remove(Book);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
