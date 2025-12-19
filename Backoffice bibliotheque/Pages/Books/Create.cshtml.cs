using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Backoffice_bibliotheque.Data;
using Backoffice_bibliotheque.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Backoffice_bibliotheque.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public IFormFile CoverImageFile { get; set; }  // Nouveau champ pour l'image

        [BindProperty]
        public List<int> SelectedAuthorIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public SelectList PublisherList { get; set; }
        public MultiSelectList AuthorList { get; set; }
        public MultiSelectList CategoryList { get; set; }

        public void OnGet()
        {
            PublisherList = new SelectList(_context.Publishers, "Id", "Name");
            AuthorList = new MultiSelectList(
                _context.Authors
                        .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                        .ToList(),
                "Id",
                "FullName"
            );
            CategoryList = new MultiSelectList(_context.Categories, "Id", "Name");
        }

        public IActionResult OnPost()
        {
            // Vérifier côté serveur les relations Many-to-Many
            if (!SelectedAuthorIds.Any())
                ModelState.AddModelError("SelectedAuthorIds", "Please select at least one author.");

            if (!SelectedCategoryIds.Any())
                ModelState.AddModelError("SelectedCategoryIds", "Please select at least one category.");

            // Gestion des noms combinés pour affichage
            Book.AuthorNamesText = string.Join(", ", _context.Authors
                                    .Where(a => SelectedAuthorIds.Contains(a.Id))
                                    .Select(a => a.FirstName + " " + a.LastName));

            Book.CategoryNamesText = string.Join(", ", _context.Categories
                                    .Where(c => SelectedCategoryIds.Contains(c.Id))
                                    .Select(c => c.Name));

            // Gestion de l'image
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

                Book.CoverImageUrl = "/uploads/" + uniqueFileName; // chemin relatif pour l'accès web
            }

            if (!ModelState.IsValid)
            {
                // Recharger les listes pour le formulaire
                OnGet();
                return Page();
            }


            _context.Books.Add(Book);
            _context.SaveChanges();

            // Relations Many-to-Many
            foreach (var authorId in SelectedAuthorIds)
            {
                _context.BookAuthors.Add(new BookAuthor { BookId = Book.Id, AuthorId = authorId });
            }

            foreach (var categoryId in SelectedCategoryIds)
            {
                _context.BookCategories.Add(new BookCategory { BookId = Book.Id, CategoryId = categoryId });
            }

            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
