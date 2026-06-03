using System.Diagnostics;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers
{
    // HomeController отговаря за началната страница.
    // Чрез конструктора получава IEventService (dependency injection).
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;

        public HomeController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Началната страница показва предстоящите събития.
        public async Task<IActionResult> Index()
        {
            var upcoming = await _eventService.GetUpcomingAsync(6);
            return View(upcoming);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
