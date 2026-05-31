using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models.ViewModels
{
    // Форма за създаване/редактиране на събитие.
    // Съдържа само полетата, които организаторът въвежда ръчно.
    public class EventFormViewModel
    {
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
        [Display(Name = "Място")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Начало")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Display(Name = "Край")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1).AddHours(2);

        [Range(1, 100000)]
        [Display(Name = "Капацитет")]
        public int Capacity { get; set; } = 100;

        [Range(0, 100000)]
        [Display(Name = "Цена (лв.)")]
        public decimal Price { get; set; }

        [Display(Name = "Снимка (URL)")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Изберете категория.")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
    }

    // Данни за формата при купуване на билет.
    public class PurchaseTicketViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }

        [Range(1, 20, ErrorMessage = "Може да купите между 1 и 20 билета.")]
        [Display(Name = "Брой билети")]
        public int Quantity { get; set; } = 1;
    }
}
