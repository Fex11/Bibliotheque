using System;
using System.Collections.Generic;

namespace Backoffice_bibliotheque.ViewModels
{
    // Modèle principal pour la page de statistiques
    public class LoanStatisticsViewModel
    {
        // Livres les plus empruntés
        public List<BookLoanCount> TopBooks { get; set; } = new();

        // Catégories les plus populaires
        public List<CategoryLoanCount> TopCategories { get; set; } = new();

        // Nombre d’emprunts par mois
        public List<MonthlyLoanCount> MonthlyLoans { get; set; } = new();
    }

    // Nombre d’emprunts par livre
    public class BookLoanCount
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int LoanCount { get; set; }
    }

    // Nombre d’emprunts par catégorie
    public class CategoryLoanCount
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int LoanCount { get; set; }
    }

    // Nombre d’emprunts par mois
    public class MonthlyLoanCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int LoanCount { get; set; }
    }
}
