using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;
using Backoffice_bibliotheque.ViewModels;

namespace Backoffice_bibliotheque.Pages.Statistics
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public LoanStatisticsViewModel Stats { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 1️Livres les plus empruntés
            Stats.TopBooks = await _context.Loans
                .GroupBy(l => l.BookId)
                .Select(g => new BookLoanCount
                {
                    BookTitle = g.FirstOrDefault()!.BookCopy!.BookTitleSnapshot,
                    LoanCount = g.Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .Take(10)
                .ToListAsync();

            var loansWithBooks = await _context.Loans
    .Include(l => l.BookCopy)
        .ThenInclude(bc => bc.Book)
    .ToListAsync();

            // 2️ Catégories les plus populaires (traitement côté client)
            Stats.TopCategories = loansWithBooks
                .Where(l => l.BookCopy?.Book?.CategoryNamesText != null)
                .SelectMany(l => l.BookCopy.Book.CategoryNamesText
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim()))
                .GroupBy(c => c)
                .Select(g => new CategoryLoanCount
                {
                    CategoryName = g.Key,
                    LoanCount = g.Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .Take(10)
                .ToList();


            // 3️ Nombre d’emprunts par mois
            Stats.MonthlyLoans = await _context.Loans
                .GroupBy(l => new { l.LoanDate.Year, l.LoanDate.Month })
                .Select(g => new MonthlyLoanCount
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    LoanCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();
        }
    }
}
