using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using EventManagementSystem.Tests.TestHelpers;
using NUnit.Framework;

namespace EventManagementSystem.Tests.Services
{
    // Тестове за EventService - търсене, филтриране и проверка на собственик.
    [TestFixture]
    public class EventServiceTests
    {
        private ApplicationDbContext _context = null!;
        private EventService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = TestDbFactory.CreateContext();
            _service = new EventService(_context);
            Seed();
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        private void Seed()
        {
            var music = new Category { Name = "Музика" };
            var sport = new Category { Name = "Спорт" };
            _context.Categories.AddRange(music, sport);

            // Създаваме и реални организатори (потребители), за да са валидни
            // връзките при Include(e => e.Organizer).
            var org1 = new ApplicationUser { Id = "org-1", UserName = "o1@a.bg", Email = "o1@a.bg", FullName = "Орг 1" };
            var org2 = new ApplicationUser { Id = "org-2", UserName = "o2@a.bg", Email = "o2@a.bg", FullName = "Орг 2" };
            _context.Users.AddRange(org1, org2);

            _context.Events.AddRange(
                new Event { Title = "Рок концерт", Description = "...", Location = "София",
                    StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(2),
                    Capacity = 100, Price = 30, Category = music, OrganizerId = "org-1" },
                new Event { Title = "Футболен мач", Description = "...", Location = "Пловдив",
                    StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(10).AddHours(2),
                    Capacity = 100, Price = 15, Category = sport, OrganizerId = "org-2" }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAll_NoFilter_ReturnsAllEvents()
        {
            var events = await _service.GetAllAsync();
            Assert.That(events.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAll_WithSearch_FiltersByTitle()
        {
            var events = await _service.GetAllAsync(search: "Рок");
            Assert.That(events.Count(), Is.EqualTo(1));
            Assert.That(events.First().Title, Is.EqualTo("Рок концерт"));
        }

        [Test]
        public async Task GetAll_WithSearchByLocation_Works()
        {
            var events = await _service.GetAllAsync(search: "Пловдив");
            Assert.That(events.Single().Title, Is.EqualTo("Футболен мач"));
        }

        [Test]
        public async Task IsOwner_ReturnsTrue_ForCorrectOrganizer()
        {
            var anyEvent = (await _service.GetAllAsync()).First(e => e.OrganizerId == "org-1");
            var isOwner = await _service.IsOwnerAsync(anyEvent.Id, "org-1");
            Assert.That(isOwner, Is.True);
        }

        [Test]
        public async Task IsOwner_ReturnsFalse_ForOtherUser()
        {
            var anyEvent = (await _service.GetAllAsync()).First();
            var isOwner = await _service.IsOwnerAsync(anyEvent.Id, "someone-else");
            Assert.That(isOwner, Is.False);
        }
    }
}
