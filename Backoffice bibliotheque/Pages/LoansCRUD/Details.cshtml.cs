using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.LoansCRUD
{
    public class DetailsModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public DetailsModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Loan Loan { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Loans == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }
            else 
            {
                Loan = loan;
            }
            return Page();
        }
    }
}
