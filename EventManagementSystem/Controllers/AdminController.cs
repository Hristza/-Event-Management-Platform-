using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Controllers
{
    // Административен панел - достъпен САМО за роля "Admin".
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Табло със статистики: брой събития, потребители, продадени билети, приходи.
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalEvents = await _context.Events.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalTicketsSold = await _context.Tickets.SumAsync(t => (int?)t.Quantity) ?? 0,
                TotalRevenue = await _context.Tickets.SumAsync(t => (decimal?)t.TotalPrice) ?? 0m
            };
            return View(model);
        }

        // Списък с всички потребители и техните роли.
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var rows = new List<UserRowViewModel>();
            foreach (var u in users)
            {
                rows.Add(new UserRowViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    FullName = u.FullName,
                    Roles = await _userManager.GetRolesAsync(u)
                });
            }
            return View(rows);
        }

        // Прави избран потребител организатор (добавя му роля "Organizer").
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeOrganizer(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && !await _userManager.IsInRoleAsync(user, DbSeeder.OrganizerRole))
            {
                await _userManager.AddToRoleAsync(user, DbSeeder.OrganizerRole);
                TempData["Success"] = $"{user.Email} вече е организатор.";
            }
            return RedirectToAction(nameof(Users));
        }
    }
}
