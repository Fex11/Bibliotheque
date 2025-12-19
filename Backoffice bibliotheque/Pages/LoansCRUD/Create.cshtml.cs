using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.LoansCRUD
{
    public class CreateModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public CreateModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
        ViewData["BookCopyId"] = new SelectList(_context.BookCopies, "Id", "Barcode");
        ViewData["BorrowerId"] = new SelectList(_context.LibraryUsers, "Id", "Email");
            return Page();
        }

        [BindProperty]
        public Loan Loan { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Loans == null || Loan == null)
            {
                return Page();
            }

            _context.Loans.Add(Loan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
