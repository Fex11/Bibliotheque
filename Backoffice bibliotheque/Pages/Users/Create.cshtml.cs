using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Users
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
            return Page();
        }

        [BindProperty]
        public LibraryUser LibraryUser { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.LibraryUsers == null || LibraryUser == null)
            {
                return Page();
            }

            _context.LibraryUsers.Add(LibraryUser);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
