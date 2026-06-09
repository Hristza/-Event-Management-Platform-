using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    // Малък клас за резултата от покупка: успех/неуспех + съобщение.
    // Така контролерът разбира какво да каже на потребителя.
    // НЕ е имплементация на интерфейс - просто носи данни (резултат).
    public class PurchaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Ticket? Ticket { get; set; }

        public static PurchaseResult Fail(string message) =>
            new() { Success = false, Message = message };

        public static PurchaseResult Ok(Ticket ticket) =>
            new() { Success = true, Ticket = ticket, Message = "Покупката е успешна!" };
    }
}
