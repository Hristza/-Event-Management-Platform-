using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Services
{
    // EventService съдържа цялата логика около събитията.
    // Контролерите само го извикват - не пишат заявки към базата сами.
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync(string? search = null, int? categoryId = null, bool includePast = false)
        {
            // IQueryable ни позволява да "сглобим" заявката стъпка по стъпка,
            // и чак накрая EF Core я превръща в един SQL към базата.
            IQueryable<Event> query = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Tickets);

            // По подразбиране показваме само ПРЕДСТОЯЩИТЕ събития, за да не
            // се "замърсява" каталогът с минали. Същото правило като IsUpcoming.
            if (!includePast)
            {
                var now = DateTime.UtcNow;
                query = query.Where(e => e.StartDate > now);
            }

            // Филтър по текст в заглавието или мястото.
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(e =>
                    e.Title.Contains(search) || e.Location.Contains(search));
            }

            // Филтър по категория.
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingAsync(int count = 6)
        {
            var now = DateTime.UtcNow;
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Tickets)
                .Where(e => e.StartDate > now)
                .OrderBy(e => e.StartDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Event?> GetDetailsAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Tickets)
                .Include(e => e.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetByOrganizerAsync(string organizerId)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Tickets)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<Event> CreateAsync(Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public async Task<bool> UpdateAsync(Event updatedEvent)
        {
            _context.Events.Update(updatedEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return false;

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsOwnerAsync(int eventId, string userId)
        {
            return await _context.Events
                .AnyAsync(e => e.Id == eventId && e.OrganizerId == userId);
        }

        public async Task<bool> AddReviewAsync(Review review)
        {
            // Не записваме мнение за несъществуващо събитие.
            var eventExists = await _context.Events.AnyAsync(e => e.Id == review.EventId);
            if (!eventExists)
                return false;

            // Един потребител оставя само едно мнение за дадено събитие.
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.EventId == review.EventId && r.UserId == review.UserId);
            if (alreadyReviewed)
                return false;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
