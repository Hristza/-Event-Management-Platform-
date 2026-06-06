using EventManagementSystem.Models;
using EventManagementSystem.Models.ViewModels;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers
{
    // Всички действия с билети изискват потребителят да е ВЛЯЗЪЛ.
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(
            ITicketService ticketService,
            IEventService eventService,
            UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _eventService = eventService;
            _userManager = userManager;
        }

        // "Моите билети" - списък с купените билети.
        public async Task<IActionResult> MyTickets()
        {
            var userId = _userManager.GetUserId(User)!;
            var tickets = await _ticketService.GetByUserAsync(userId);
            return View(tickets);
        }

        // Buy (GET): показва форма за избор на брой билети.
        [HttpGet]
        public async Task<IActionResult> Buy(int eventId)
        {
            var ev = await _eventService.GetDetailsAsync(eventId);
            if (ev == null)
                return NotFound();

            var model = new PurchaseTicketViewModel
            {
                EventId = ev.Id,
                EventTitle = ev.Title,
                Price = ev.Price,
                AvailableSeats = ev.AvailableSeats
            };
            return View(model);
        }

        // Buy (POST): извиква бизнес логиката в TicketService.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(PurchaseTicketViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User)!;
            var result = await _ticketService.PurchaseAsync(model.EventId, userId, model.Quantity);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["Success"] = $"{result.Message} Код: {result.Ticket!.TicketCode}";
            return RedirectToAction(nameof(MyTickets));
        }

        // Cancel (POST): отказ на билет.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var ok = await _ticketService.CancelAsync(id, userId);
            TempData[ok ? "Success" : "Error"] =
                ok ? "Билетът е върнат." : "Билетът не може да бъде върнат.";
            return RedirectToAction(nameof(MyTickets));
        }
    }
}
