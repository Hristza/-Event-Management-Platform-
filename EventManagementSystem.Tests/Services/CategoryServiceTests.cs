using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using EventManagementSystem.Tests.TestHelpers;
using NUnit.Framework;

namespace EventManagementSystem.Tests.Services
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private ApplicationDbContext _context = null!;
        private CategoryService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = TestDbFactory.CreateContext();
            _service = new CategoryService(_context);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task Create_AddsCategory()
        {
            await _service.CreateAsync(new Category { Name = "Кино" });
            var all = await _service.GetAllAsync();
            Assert.That(all.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Delete_EmptyCategory_Succeeds()
        {
            var created = await _service.CreateAsync(new Category { Name = "Празна" });
            var result = await _service.DeleteAsync(created.Id);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Delete_CategoryWithEvents_IsRejected()
        {
            // Бизнес правило: категория със събития НЕ може да се изтрие.
            var category = new Category { Name = "Заета" };
            category.Events.Add(new Event
            {
                Title = "Събитие", Description = "...", Location = "София",
                StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
                Capacity = 10, Price = 5, OrganizerId = "org-1"
            });
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(category.Id);

            Assert.That(result, Is.False, "Не трябва да изтрием категория със събития.");
            Assert.That(await _service.ExistsAsync(category.Id), Is.True);
        }
    }
}
