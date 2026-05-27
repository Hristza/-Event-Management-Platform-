using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    // Малък клас за резултата от покупка: успех/неуспех + съобщение.
    // Така контролерът разбира какво да каже на потребителя.
    public class PurchaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Ticket? Ticket { get; set; }

        public static PurchaseResult Fail(string message) =>
            new() { Success = false, Message = message };

        public static PurchaseResult Ok(Ticket ticket) =>
            new() { Success = true, Ticket = ticket, Message = "Покупката е успешна!" };
    }

    public interface ITicketService
    {
        // Купуване на билет(и) - тук е най-важната бизнес логика.
        Task<PurchaseResult> PurchaseAsync(int eventId, string userId, int quantity);

        // Билетите на конкретен потребител.
        Task<IEnumerable<Ticket>> GetByUserAsync(string userId);

        // Един билет с детайли.
        Task<Ticket?> GetByIdAsync(int id);

        // Отказване/връщане на билет.
        Task<bool> CancelAsync(int ticketId, string userId);
    }
}
