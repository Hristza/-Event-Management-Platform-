using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Models.ViewModels;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagementSystem.Controllers
{
    // EventsController прави CRUD (Create, Read, Update, Delete) върху събитията.
    // Тук ясно се вижда работата с РОЛИ чрез атрибута [Authorize].
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(
            IEventService eventService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        // READ: каталог с всички събития + търсене и филтър. Достъпно за всички.
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int? categoryId, bool includePast = false)
        {
            var events = await _eventService.GetAllAsync(search, categoryId, includePast);
            ViewBag.Categories = await BuildCategorySelectList(categoryId);
            ViewBag.Search = search;
            ViewBag.IncludePast = includePast;
            return View(events);
        }

        // READ: подробности за едно събитие. Достъпно за всички.
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();
            return View(ev);
        }

        // "Моите събития" - само за организатори/админи.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> MyEvents()
        {
            var userId = _userManager.GetUserId(User)!;
            var events = await _eventService.GetByOrganizerAsync(userId);
            return View(events);
        }

        // CREATE (GET): показва празна форма. Само организатори/админи.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await BuildCategorySelectList(null);
            return View(new EventFormViewModel());
        }

        // CREATE (POST): записва новото събитие.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            ValidateDates(model);
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await BuildCategorySelectList(model.CategoryId);
                return View(model);
            }

            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Capacity = model.Capacity,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                OrganizerId = _userManager.GetUserId(User)!
            };

            await _eventService.CreateAsync(newEvent);
            TempData["Success"] = "Събитието е създадено успешно!";
            return RedirectToAction(nameof(MyEvents));
        }

        // UPDATE (GET): зарежда формата с данните на събитието.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();

            if (!await CanModify(id))
                return Forbid();

            var model = new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Capacity = ev.Capacity,
                Price = ev.Price,
                ImageUrl = ev.ImageUrl,
                CategoryId = ev.CategoryId
            };
            ViewBag.Categories = await BuildCategorySelectList(model.CategoryId);
            return View(model);
        }

        // UPDATE (POST): записва промените.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(EventFormViewModel model)
        {
            ValidateDates(model);
            if (!await CanModify(model.Id))
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await BuildCategorySelectList(model.CategoryId);
                return View(model);
            }

            var ev = await _eventService.GetDetailsAsync(model.Id);
            if (ev == null)
                return NotFound();

            ev.Title = model.Title;
            ev.Description = model.Description;
            ev.Location = model.Location;
            ev.StartDate = model.StartDate;
            ev.EndDate = model.EndDate;
            ev.Capacity = model.Capacity;
            ev.Price = model.Price;
            ev.ImageUrl = model.ImageUrl;
            ev.CategoryId = model.CategoryId;

            await _eventService.UpdateAsync(ev);
            TempData["Success"] = "Промените са запазени.";
            return RedirectToAction(nameof(MyEvents));
        }

        // DELETE (GET): страница за потвърждение.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _eventService.GetDetailsAsync(id);
            if (ev == null)
                return NotFound();
            if (!await CanModify(id))
                return Forbid();
            return View(ev);
        }

        // DELETE (POST): реално изтриване.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await CanModify(id))
                return Forbid();

            await _eventService.DeleteAsync(id);
            TempData["Success"] = "Събитието е изтрито.";
            return RedirectToAction(nameof(MyEvents));
        }

        // ----- Помощни (private) методи -----

        // Админът може всичко; организаторът - само своите събития.
        private async Task<bool> CanModify(int eventId)
        {
            if (User.IsInRole(DbSeeder.AdminRole))
                return true;
            var userId = _userManager.GetUserId(User)!;
            return await _eventService.IsOwnerAsync(eventId, userId);
        }

        // Прави падащо меню (dropdown) с категориите.
        private async Task<SelectList> BuildCategorySelectList(int? selectedId)
        {
            var categories = await _categoryService.GetAllAsync();
            return new SelectList(categories, "Id", "Name", selectedId);
        }

        // Валидация: краят не може да е преди началото.
        private void ValidateDates(EventFormViewModel model)
        {
            if (model.EndDate <= model.StartDate)
                ModelState.AddModelError(nameof(model.EndDate), "Краят трябва да е след началото.");
        }
    }
}
