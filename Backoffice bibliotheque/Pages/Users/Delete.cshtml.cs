using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public DeleteModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public LibraryUser LibraryUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.LibraryUsers == null)
            {
                return NotFound();
            }

            var libraryuser = await _context.LibraryUsers.FirstOrDefaultAsync(m => m.Id == id);

            if (libraryuser == null)
            {
                return NotFound();
            }
            else 
            {
                LibraryUser = libraryuser;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.LibraryUsers == null)
            {
                return NotFound();
            }
            var libraryuser = await _context.LibraryUsers.FindAsync(id);

            if (libraryuser != null)
            {
                LibraryUser = libraryuser;
                _context.LibraryUsers.Remove(LibraryUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
