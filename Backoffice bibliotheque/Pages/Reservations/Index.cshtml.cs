using Backoffice_bibliotheque.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backoffice_bibliotheque.Pages.Reservations
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BookReservationCount> Books { get; set; } = new();

        public async Task OnGetAsync()
        {
            Books = await _context.Reservations
                .Where(r => r.Status == "pending" || r.Status == "approved")
                .GroupBy(r => new { r.BookId, r.Book.Title })
                .Select(g => new BookReservationCount
                {
                    BookId = g.Key.BookId,
                    BookTitle = g.Key.Title,
                    PendingCount = g.Count()
                })
                .OrderByDescending(x => x.PendingCount)
                .ToListAsync();
        }
    }

    public class BookReservationCount
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public int PendingCount { get; set; }
    }

}
