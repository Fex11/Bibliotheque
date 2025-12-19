using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Backoffice_bibliotheque.Pages.BookCopies
{
    public class CreateCopyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateCopyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BookCopy BookCopy { get; set; }

        public Book Book { get; set; }

        public IActionResult OnGet(int bookId)
        {
            Book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (Book == null)
                return NotFound();

            BookCopy = new BookCopy
            {
                BookId = Book.Id,
                BookTitleSnapshot = Book.Title,
                CategoryNamesSnapshot = Book.CategoryNamesText,
                AuthorNamesSnapshot = Book.AuthorNamesText,
                AcquisitionDate = DateTime.Now,
                Status = "Disponible"
            };

            return Page();
        }


        public IActionResult OnPost()
        {
            // RECHARGER LE LIVRE
            Book = _context.Books.FirstOrDefault(b => b.Id == BookCopy.BookId);

            if (Book == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return Page(); // Book n'est plus null maintenant
            }

            _context.BookCopies.Add(BookCopy);

            //  +1 TOTAL ET AVAILABLE
            Book.TotalCopiesCount += 1;
            Book.AvailableCopiesCount += 1;

            _context.SaveChanges();

            return RedirectToPage("/Books/Details", new { id = BookCopy.BookId });
        }


    }
}
