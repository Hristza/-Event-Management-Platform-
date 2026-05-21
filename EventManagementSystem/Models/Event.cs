using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.Models
{
    // Event = СЪБИТИЕ. Това е централният модел на цялата система.
    // Концерт, конференция, мач — всичко това е "Event".
    public class Event
    {
        // Първичен ключ.
        public int Id { get; set; }

        [Required(ErrorMessage = "Заглавието е задължително.")]
        [StringLength(120, MinimumLength = 3)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Място")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Начало")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Край")]
        public DateTime EndDate { get; set; }

        // Колко места общо има събитието (капацитет).
        [Range(1, 100000, ErrorMessage = "Капацитетът трябва да е поне 1.")]
        [Display(Name = "Капацитет")]
        public int Capacity { get; set; }

        // Цена на един билет. [Column(TypeName)] казва на SQL Server
        // да пази точно число с 2 знака след десетичната запетая (за пари).
        [Range(0, 100000)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Цена (лв.)")]
        public decimal Price { get; set; }

        [Display(Name = "Снимка (URL)")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // ----- Връзка с Category (много събития -> една категория) -----
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }   // навигационно свойство

        // ----- Връзка с организатора (ApplicationUser) -----
        // OrganizerId е външен ключ (Foreign Key) към таблицата с потребители.
        public string OrganizerId { get; set; } = string.Empty;
        public ApplicationUser? Organizer { get; set; }

        // ----- Връзка с билетите и ревютата -----
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // ===== Изчисляеми свойства (НЕ се пазят в базата) =====
        // [NotMapped] = "не прави колона за това в базата данни".
        // Това е малко бизнес логика директно в модела.

        // Колко билета са продадени досега (сборът от количествата).
        [NotMapped]
        public int TicketsSold => Tickets?.Sum(t => t.Quantity) ?? 0;

        // Колко свободни места са останали.
        [NotMapped]
        public int AvailableSeats => Capacity - TicketsSold;

        // Дали събитието вече е разпродадено.
        [NotMapped]
        public bool IsSoldOut => AvailableSeats <= 0;

        // Дали събитието е в бъдещето.
        [NotMapped]
        public bool IsUpcoming => StartDate > DateTime.UtcNow;
    }
}
