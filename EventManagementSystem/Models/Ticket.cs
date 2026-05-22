using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.Models
{
    // Ticket = БИЛЕТ. Когато потребител си купи място за събитие,
    // се създава един Ticket запис в базата данни.
    public class Ticket
    {
        public int Id { get; set; }

        // За кое събитие е билетът (Foreign Key + навигация).
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Кой потребител е купил билета (Foreign Key + навигация).
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // Колко билета е купил наведнъж (например 2 за него и приятел).
        [Range(1, 20)]
        [Display(Name = "Брой")]
        public int Quantity { get; set; } = 1;

        // Общата сума = цена на събитието * брой. Изчислява се в сервиза.
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Обща сума")]
        public decimal TotalPrice { get; set; }

        // Уникален код на билета (като баркод). Генерира се автоматично.
        [Display(Name = "Код на билета")]
        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N")[..10].ToUpper();

        [Display(Name = "Купен на")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }
}
