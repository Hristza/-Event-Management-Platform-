namespace EventManagementSystem.Models.ViewModels
{
    // Данни за административното табло (Dashboard) - само числа за справка.
    public class AdminDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Един ред в списъка с потребители (за управление на роли).
    public class UserRowViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
