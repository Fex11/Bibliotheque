using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;

namespace Backoffice_bibliotheque.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public DashboardStats Stats { get; set; } = new();

        public async Task OnGetAsync()
        {
            var now = DateTime.Now;

            // 📘 Emprunts en cours
            Stats.ActiveLoans = await _context.Loans
                .CountAsync(l => l.Status == "onloan");

            // ⏰ Emprunts en retard
            Stats.LateLoans = await _context.Loans
                .CountAsync(l =>
                    l.Status == "onloan" &&
                    l.DueDate < now);

            // 📌 Réservations expirées
            Stats.ExpiredReservations = await _context.Reservations
                .CountAsync(r =>
                    r.Status == "approved" &&
                    r.ExpireAt != null &&
                    r.ExpireAt < now);
        }
    }
}
