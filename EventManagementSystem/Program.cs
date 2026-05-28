using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1) РЕГИСТРАЦИЯ НА УСЛУГИ (services) в DI контейнера.
// ============================================================

// 1.1) Базата данни (Entity Framework Core + SQL Server).
// Взимаме connection string-а от appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Липсва connection string 'DefaultConnection'.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 1.2) ASP.NET Identity - системата за потребители, пароли и РОЛИ.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // По-облекчени правила за паролите, удобни за демонстрация.
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()                      // включваме ролите
    .AddEntityFrameworkStores<ApplicationDbContext>(); // пазим всичко в нашата база

// 1.3) Къде да пренасочва, ако някой не е влязъл или няма права.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 1.4) НАШИТЕ сервизи (бизнес логиката). "Scoped" = един екземпляр
// за всяка HTTP заявка. Това е dependency injection в действие.
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// 1.5) MVC контролери + изгледи (Views).
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ============================================================
// 2) HTTP PIPELINE - редът, по който минава всяка заявка.
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // позволява зареждане на CSS, JS, снимки от wwwroot

app.UseRouting();

// ВАЖНО: първо Authentication (кой си ти?), после Authorization (какво ти е позволено?).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============================================================
// 3) ЗАСЯВАНЕ НА БАЗАТА с начални данни при стартиране.
// ============================================================
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
