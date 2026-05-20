using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    // Category = категория на събитие, например "Музика", "Спорт", "Конференция".
    // Едно събитие принадлежи към една категория.
    public class Category
    {
        // Първичен ключ (Primary Key). EF Core автоматично разбира, че
        // свойство на име "Id" е ключът на таблицата.
        public int Id { get; set; }

        // Името на категорията. [Required] = задължително поле.
        // [MaxLength] ограничава дължината в базата данни.
        [Required(ErrorMessage = "Името на категорията е задължително.")]
        [MaxLength(60)]
        [Display(Name = "Категория")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        // Навигация: всички събития в тази категория (one-to-many).
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
