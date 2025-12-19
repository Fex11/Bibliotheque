using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Roles
{
    public class DetailsModel : PageModel
    {
        private readonly Backoffice_bibliotheque.Data.ApplicationDbContext _context;

        public DetailsModel(Backoffice_bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Role Role { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            else 
            {
                Role = role;
            }
            return Page();
        }
    }
}
