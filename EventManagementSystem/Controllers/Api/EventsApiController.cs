using EventManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    // Това е API контролер - връща ДАННИ (JSON), а не HTML страници.
    // [ApiController] добавя автоматична валидация и удобства.
    // Маршрутът е /api/events.
    [ApiController]
    [Route("api/events")]
    [Produces("application/json")]
    public class EventsApiController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsApiController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET /api/events  -> връща списък със събития (като JSON).
        // По избор: /api/events?search=рок&categoryId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? categoryId)
        {
            var events = await _eventService.GetAllAsync(search, categoryId);

            // Връщаме само нужните полета (за да не издаваме излишни данни).
            var result = events.Select(e => new
            {
                e.Id,
                e.Title,
                e.Location,
                e.StartDate,
                e.Price,
                Category = e.Category?.Name,
                AvailableSeats = e.AvailableSeats,
                IsSoldOut = e.IsSoldOut
            });

            return Ok(result);
        }

        // GET /api/events/5  -> връща едно събитие или 404, ако го няма.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _eventService.GetDetailsAsync(id);
            if (e == null)
                return NotFound(new { message = "Събитието не е намерено." });

            return Ok(new
            {
                e.Id,
                e.Title,
                e.Description,
                e.Location,
                e.StartDate,
                e.EndDate,
                e.Price,
                e.Capacity,
                AvailableSeats = e.AvailableSeats,
                Category = e.Category?.Name,
                Organizer = e.Organizer?.FullName
            });
        }
    }
}
