using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public EditModel(Bibliotheque.Data.ApplicationDbContext context)
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

            var libraryuser =  await _context.LibraryUsers.FirstOrDefaultAsync(m => m.Id == id);
            if (libraryuser == null)
            {
                return NotFound();
            }
            LibraryUser = libraryuser;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LibraryUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryUserExists(LibraryUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LibraryUserExists(int id)
        {
          return (_context.LibraryUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
