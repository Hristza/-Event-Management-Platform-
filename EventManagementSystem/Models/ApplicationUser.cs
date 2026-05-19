using Microsoft.AspNetCore.Identity;

namespace EventManagementSystem.Models
{
    // ApplicationUser е НАШИЯТ потребител. Той наследява IdentityUser,
    // което вече ни дава готови полета: Email, UserName, PasswordHash и т.н.
    // Ние просто добавяме допълнителните неща, които ни трябват за проекта.
    public class ApplicationUser : IdentityUser
    {
        // Пълното име на потребителя (например "Иван Петров").
        [PersonalData]
        public string FullName { get; set; } = string.Empty;

        // Кога потребителят се е регистрирал.
        public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;

        // Навигация: всички билети, които този потребител е купил.
        // "Един потребител има много билети" (one-to-many).
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        // Навигация: всички събития, които този потребител е организирал.
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

        // Навигация: всички ревюта/мнения, които потребителят е оставил.
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
