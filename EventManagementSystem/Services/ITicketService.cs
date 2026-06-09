using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    // Този файл съдържа САМО интерфейса (договора).
    // Имплементацията е в TicketService.cs, а резултатът - в PurchaseResult.cs.
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
