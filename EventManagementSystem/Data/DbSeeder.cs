using EventManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data
{
    // DbSeeder "засява" базата с начални данни: роли, потребители,
    // категории и няколко примерни събития. Така на защитата системата
    // НЕ е празна и веднага има какво да се покаже.
    public static class DbSeeder
    {
        // Имената на ролите държим на едно място, за да не грешим правописа.
        public const string AdminRole = "Admin";
        public const string OrganizerRole = "Organizer";
        public const string UserRole = "User";

        public static async Task SeedAsync(IServiceProvider services)
        {
            // Взимаме нужните "инструменти" от dependency injection контейнера.
            var context = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Прилагаме всички миграции (създава базата, ако я няма).
            await context.Database.MigrateAsync();

            // 2) Създаваме трите роли, ако още ги няма.
            foreach (var role in new[] { AdminRole, OrganizerRole, UserRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 3) Създаваме по един потребител за всяка роля.
            var admin = await EnsureUserAsync(userManager, "admin@events.bg", "Admin123!", "Главен Администратор", AdminRole);
            var organizer = await EnsureUserAsync(userManager, "organizer@events.bg", "Organizer123!", "Орлин Организатор", OrganizerRole);
            await EnsureUserAsync(userManager, "user@events.bg", "User123!", "Потребител Потребителов", UserRole);

            // 4) Ако вече има събития, не дублираме данните.
            if (await context.Events.AnyAsync())
                return;

            // 5) Категории.
            var music = new Category { Name = "Музика", Description = "Концерти и фестивали" };
            var sport = new Category { Name = "Спорт", Description = "Мачове и турнири" };
            var tech = new Category { Name = "Технологии", Description = "Конференции и хакатони" };
            context.Categories.AddRange(music, sport, tech);
            await context.SaveChangesAsync();

            // 6) Примерни събития, организирани от "organizer".
            var events = new List<Event>
            {
                new Event
                {
                    Title = "Рок концерт на годината",
                    Description = "Най-голямото рок събитие с водещи български групи на открита сцена.",
                    Location = "София, НДК",
                    StartDate = DateTime.UtcNow.AddDays(14),
                    EndDate = DateTime.UtcNow.AddDays(14).AddHours(4),
                    Capacity = 500,
                    Price = 45.00m,
                    CategoryId = music.Id,
                    OrganizerId = organizer.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800"
                },
                new Event
                {
                    Title = "Градски маратон 2026",
                    Description = "Любителско бягане на 10 км през центъра на града. Медал за всеки финиширал.",
                    Location = "Пловдив, Гребна база",
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(30).AddHours(6),
                    Capacity = 1000,
                    Price = 20.00m,
                    CategoryId = sport.Id,
                    OrganizerId = organizer.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1452626038306-9aae5e071dd3?w=800"
                },
                new Event
                {
                    Title = "Tech Conference: AI и бъдещето",
                    Description = "Конференция за изкуствен интелект, разработка и кариера в IT сектора.",
                    Location = "Варна, Технопарк",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(7).AddHours(8),
                    Capacity = 200,
                    Price = 60.00m,
                    CategoryId = tech.Id,
                    OrganizerId = organizer.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800"
                }
            };
            context.Events.AddRange(events);
            await context.SaveChangesAsync();
        }

        // Помощен метод: създава потребител САМО ако още не съществува,
        // задава му парола и роля. Връща готовия потребител.
        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email, string password, string fullName, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName
                };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
            return user;
        }
    }
}
