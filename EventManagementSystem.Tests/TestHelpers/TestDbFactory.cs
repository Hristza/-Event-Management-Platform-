using EventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Tests.TestHelpers
{
    // Малък помощник: създава НОВА база в паметта (InMemory) за всеки тест.
    // Така тестовете са бързи и независими един от друг - не пипат реалния SQL Server.
    public static class TestDbFactory
    {
        public static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                // Уникално име => всеки тест има собствена чиста база.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
