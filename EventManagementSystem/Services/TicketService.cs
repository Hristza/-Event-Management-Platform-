using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        // НАЙ-ВАЖНИЯТ метод в проекта. Стъпки:
        // 1) Намираме събитието и вече продадените билети.
        // 2) Проверяваме дали количеството е валидно.
        // 3) Проверяваме дали има достатъчно свободни места (без презаписване!).
        // 4) Изчисляваме общата цена.
        // 5) Записваме билета в базата.
        public async Task<PurchaseResult> PurchaseAsync(int eventId, string userId, int quantity)
        {
            if (quantity < 1)
                return PurchaseResult.Fail("Бройката трябва да е поне 1.");

            var ev = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return PurchaseResult.Fail("Събитието не съществува.");

            if (ev.StartDate <= DateTime.UtcNow)
                return PurchaseResult.Fail("Събитието вече е минало или е започнало.");

            // Колко са продадени до момента + колко иска сега.
            int alreadySold = ev.Tickets.Sum(t => t.Quantity);
            int seatsLeft = ev.Capacity - alreadySold;

            if (quantity > seatsLeft)
                return PurchaseResult.Fail($"Няма достатъчно места. Свободни: {seatsLeft}.");

            // Изчисляваме сумата (цена * брой).
            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                Quantity = quantity,
                TotalPrice = ev.Price * quantity,
                PurchaseDate = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return PurchaseResult.Ok(ticket);
        }

        public async Task<IEnumerable<Ticket>> GetByUserAsync(string userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                    .ThenInclude(e => e!.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Бизнес правило: потребител може да откаже САМО свой билет
        // и само ако събитието още не е започнало.
        public async Task<bool> CancelAsync(int ticketId, string userId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || ticket.UserId != userId)
                return false;

            if (ticket.Event != null && ticket.Event.StartDate <= DateTime.UtcNow)
                return false; // събитието вече е започнало

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
