using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using EventManagementSystem.Tests.TestHelpers;
using NUnit.Framework;

namespace EventManagementSystem.Tests.Services
{
    // Тестове за TicketService - тук е НАЙ-важната бизнес логика:
    // изчисляване на цена и предпазване от презаписване (overbooking).
    [TestFixture]
    public class TicketServiceTests
    {
        private ApplicationDbContext _context = null!;
        private TicketService _service = null!;

        // [SetUp] се изпълнява ПРЕДИ всеки тест - подготвя чиста среда.
        [SetUp]
        public void SetUp()
        {
            _context = TestDbFactory.CreateContext();
            _service = new TicketService(_context);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        // Помощен метод: създава едно събитие с даден капацитет и цена.
        private async Task<Event> SeedEventAsync(int capacity, decimal price)
        {
            var category = new Category { Name = "Тест" };
            var ev = new Event
            {
                Title = "Тестово събитие",
                Description = "Описание за теста",
                Location = "София",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(5).AddHours(2),
                Capacity = capacity,
                Price = price,
                Category = category,
                OrganizerId = "organizer-1"
            };
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return ev;
        }

        [Test]
        public async Task Purchase_WithEnoughSeats_Succeeds_AndCalculatesPrice()
        {
            // Arrange (подготовка)
            var ev = await SeedEventAsync(capacity: 100, price: 25m);

            // Act (действие)
            var result = await _service.PurchaseAsync(ev.Id, "user-1", quantity: 3);

            // Assert (проверка)
            Assert.That(result.Success, Is.True);
            Assert.That(result.Ticket, Is.Not.Null);
            Assert.That(result.Ticket!.TotalPrice, Is.EqualTo(75m)); // 25 * 3
        }

        [Test]
        public async Task Purchase_MoreThanCapacity_Fails()
        {
            // Само 2 места, искаме 5 -> трябва да откаже.
            var ev = await SeedEventAsync(capacity: 2, price: 10m);

            var result = await _service.PurchaseAsync(ev.Id, "user-1", quantity: 5);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("места"));
        }

        [Test]
        public async Task Purchase_PreventsOverbooking_AcrossMultipleBuyers()
        {
            // Капацитет 5. Първият купува 4, вторият иска 3 -> не трябва да може.
            var ev = await SeedEventAsync(capacity: 5, price: 10m);

            var first = await _service.PurchaseAsync(ev.Id, "user-1", 4);
            var second = await _service.PurchaseAsync(ev.Id, "user-2", 3);

            Assert.That(first.Success, Is.True);
            Assert.That(second.Success, Is.False, "Не трябва да продаваме повече от капацитета.");
        }

        [Test]
        public async Task Purchase_NonExistingEvent_Fails()
        {
            var result = await _service.PurchaseAsync(eventId: 999, "user-1", 1);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task Purchase_ZeroQuantity_Fails()
        {
            var ev = await SeedEventAsync(capacity: 10, price: 10m);
            var result = await _service.PurchaseAsync(ev.Id, "user-1", 0);
            Assert.That(result.Success, Is.False);
        }
    }
}
