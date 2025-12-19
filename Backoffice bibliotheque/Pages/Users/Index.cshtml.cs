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
    public class IndexModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public IndexModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LibraryUser> LibraryUser { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.LibraryUsers != null)
            {
                LibraryUser = await _context.LibraryUsers.ToListAsync();
            }
        }
    }
}
