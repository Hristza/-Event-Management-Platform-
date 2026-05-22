using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    // Review = МНЕНИЕ/ОЦЕНКА, която потребител оставя за дадено събитие.
    // Това е бонус модел, който показва връзка "много към много" през
    // отделна таблица (потребител оценява събитие).
    public class Review
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // Оценка от 1 до 5 звезди.
        [Range(1, 5, ErrorMessage = "Оценката е между 1 и 5.")]
        [Display(Name = "Оценка")]
        public int Rating { get; set; }

        [MaxLength(500)]
        [Display(Name = "Коментар")]
        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
