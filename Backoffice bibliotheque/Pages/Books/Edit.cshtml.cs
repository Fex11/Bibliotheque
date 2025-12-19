using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bibliotheque.Data;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace Backoffice_bibliotheque.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public IFormFile CoverImageFile { get; set; }

        [BindProperty]
        public List<int> SelectedAuthorIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public SelectList PublisherList { get; set; }
        public MultiSelectList AuthorList { get; set; }
        public MultiSelectList CategoryList { get; set; }

        public IActionResult OnGet(int id)
        {
            Book = _context.Books
                    .Include(b => b.Publisher)
                    .FirstOrDefault(b => b.Id == id);

            if (Book == null)
                return NotFound();

            PublisherList = new SelectList(_context.Publishers, "Id", "Name", Book.PublisherId);

            AuthorList = new MultiSelectList(
                _context.Authors
                    .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                    .ToList(),
                "Id",
                "FullName",
                _context.BookAuthors.Where(ba => ba.BookId == id).Select(ba => ba.AuthorId)
            );

            CategoryList = new MultiSelectList(
                _context.Categories,
                "Id",
                "Name",
                _context.BookCategories.Where(bc => bc.BookId == id).Select(bc => bc.CategoryId)
            );

            SelectedAuthorIds = _context.BookAuthors.Where(ba => ba.BookId == id).Select(ba => ba.AuthorId).ToList();
            SelectedCategoryIds = _context.BookCategories.Where(bc => bc.BookId == id).Select(bc => bc.CategoryId).ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!SelectedAuthorIds.Any())
                ModelState.AddModelError("SelectedAuthorIds", "Please select at least one author.");

            if (!SelectedCategoryIds.Any())
                ModelState.AddModelError("SelectedCategoryIds", "Please select at least one category.");

            if (!ModelState.IsValid)
            {
                OnGet(Book.Id);
                return Page();
            }

            var bookToUpdate = _context.Books
                                .Include(b => b.Publisher)
                                .FirstOrDefault(b => b.Id == Book.Id);

            if (bookToUpdate == null)
                return NotFound();

            // Mise à jour des champs simples
            bookToUpdate.Title = Book.Title;
            bookToUpdate.Subtitle = Book.Subtitle;
            bookToUpdate.PublicationYear = Book.PublicationYear;
            bookToUpdate.PublisherId = Book.PublisherId;
            bookToUpdate.Keyword = Book.Keyword;

            // Gestion image
            if (CoverImageFile != null && CoverImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    CoverImageFile.CopyTo(fileStream);
                }

                bookToUpdate.CoverImageUrl = "/uploads/" + uniqueFileName;
            }

            // Relations Many-to-Many
            // Supprimer anciennes
            var oldAuthors = _context.BookAuthors.Where(ba => ba.BookId == bookToUpdate.Id);
            _context.BookAuthors.RemoveRange(oldAuthors);

            var oldCategories = _context.BookCategories.Where(bc => bc.BookId == bookToUpdate.Id);
            _context.BookCategories.RemoveRange(oldCategories);

            // Ajouter nouvelles
            foreach (var authorId in SelectedAuthorIds)
            {
                _context.BookAuthors.Add(new BookAuthor { BookId = bookToUpdate.Id, AuthorId = authorId });
            }

            foreach (var categoryId in SelectedCategoryIds)
            {
                _context.BookCategories.Add(new BookCategory { BookId = bookToUpdate.Id, CategoryId = categoryId });
            }

            // Mettre à jour les champs combinés pour affichage
            bookToUpdate.AuthorNamesText = string.Join(", ", _context.Authors
                                                .Where(a => SelectedAuthorIds.Contains(a.Id))
                                                .Select(a => a.FirstName + " " + a.LastName));

            bookToUpdate.CategoryNamesText = string.Join(", ", _context.Categories
                                                .Where(c => SelectedCategoryIds.Contains(c.Id))
                                                .Select(c => c.Name));

            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
