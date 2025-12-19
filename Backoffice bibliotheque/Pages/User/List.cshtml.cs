using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backoffice_bibliotheque.Pages.User
{
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<LibraryUser> LibraryUsers { get; set; } = new List<LibraryUser>();

        public async Task OnGetAsync()
        {
            LibraryUsers = await _context.LibraryUsers.ToListAsync();
        }
    }
}
