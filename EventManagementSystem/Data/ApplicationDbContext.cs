using EventManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data
{
    // ApplicationDbContext е "мостът" между C# кода и базата данни.
    // Той наследява IdentityDbContext, за да получи наготово всички
    // таблици за потребители, роли и пароли (ASP.NET Identity).
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Всеки DbSet се превръща в таблица в базата данни.
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Review> Reviews => Set<Review>();

        // Тук с "Fluent API" настройваме връзките и правилата по-точно.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ВАЖНО: при Identity винаги първо извикваме base, иначе
            // таблиците за потребители няма да се конфигурират правилно.
            base.OnModelCreating(builder);

            // Event -> Category: ако изтрием категория, НЕ трием събитията
            // (Restrict), за да не загубим данни случайно.
            builder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event -> Organizer (потребител).
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> Event: ако се изтрие събитие, се трият и билетите му (Cascade).
            builder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ticket -> User.
            builder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review -> Event и Review -> User.
            builder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
