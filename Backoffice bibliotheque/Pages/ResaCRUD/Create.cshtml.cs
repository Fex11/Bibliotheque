using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bibliotheque.Data;
using Bibliotheque.Models;
using Microsoft.EntityFrameworkCore;

namespace Backoffice_bibliotheque.Pages.ResaCRUD
{
    public class CreateModel : PageModel
    {
        private readonly Bibliotheque.Data.ApplicationDbContext _context;

        public CreateModel(Bibliotheque.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
        ViewData["RequesterId"] = new SelectList(_context.LibraryUsers, "Id", "Email");
            return Page();
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 🔎 Charger Book et User
            var book = await _context.Books.FindAsync(Reservation.BookId);
            var user = await _context.LibraryUsers.FindAsync(Reservation.RequesterId);

            if (book == null || user == null)
            {
                ModelState.AddModelError("", "Invalid book or user");
                return Page();
            }

            // 🔢 Calcul de la position dans la file (pending + approved)
            var lastPosition = await _context.Reservations
                .Where(r => r.BookId == Reservation.BookId
                         && (r.Status == "pending" || r.Status == "approved"))
                .Select(r => (int?)r.PositionInQueue)
                .MaxAsync() ?? 0;

            Reservation.PositionInQueue = lastPosition + 1;

            // 🔍 Vérifier s’il existe une copie disponible
            var availableCopy = await _context.BookCopies
                .Where(c => c.BookId == Reservation.BookId && c.Status == "Disponible")
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            bool hasAvailableCopy = availableCopy != null;

            //📌 Statut basé UNIQUEMENT sur la disponibilité
            Reservation.Status = hasAvailableCopy ? "approved" : "pending";

            // 🕒 Dates
            Reservation.RequestedAt = DateTime.Now;
            Reservation.CreatedAt = DateTime.Now;
            Reservation.UpdatedAt = DateTime.Now;

            if (Reservation.Status == "approved")
            {
                Reservation.ExpireAt = DateTime.Now.AddDays(2); // ex: 48h
                availableCopy.Status = "Reserved";
            }
            else
            {
                Reservation.ExpireAt = null;
            }

            // 📸 Snapshots
            Reservation.BookTitleSnapshot = book.Title;
            Reservation.RequesterNameSnapshot = $"{user.FirstName} {user.LastName}";

            // 💾 Sauvegarde
            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }


    }
}
