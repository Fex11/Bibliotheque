using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.Data;
using Bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public IndexModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Role> Role { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Roles != null)
            {
                Role = await _context.Roles.ToListAsync();
            }
        }
    }
}
