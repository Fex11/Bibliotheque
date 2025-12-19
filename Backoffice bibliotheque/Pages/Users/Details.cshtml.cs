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
    public class DetailsModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public DetailsModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
