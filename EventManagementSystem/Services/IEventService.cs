using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public interface IEventService
    {
        // Списък с филтър по търсене и категория (за началната страница и каталога).
        Task<IEnumerable<Event>> GetAllAsync(string? search = null, int? categoryId = null);

        // Само предстоящите събития (за началната страница).
        Task<IEnumerable<Event>> GetUpcomingAsync(int count = 6);

        // Едно събитие с всичките му връзки (категория, организатор, билети).
        Task<Event?> GetDetailsAsync(int id);

        // Събитията на конкретен организатор.
        Task<IEnumerable<Event>> GetByOrganizerAsync(string organizerId);

        Task<Event> CreateAsync(Event newEvent);
        Task<bool> UpdateAsync(Event updatedEvent);
        Task<bool> DeleteAsync(int id);

        // Проверка дали даден потребител е собственик/организатор на събитие.
        Task<bool> IsOwnerAsync(int eventId, string userId);
    }
}
