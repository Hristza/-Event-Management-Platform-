using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models.ViewModels
{
    // ViewModel = малък клас, който носи точно данните за един екран/форма.
    // Не използваме директно ApplicationUser във формите по съображения за
    // сигурност и яснота.

    // Данни за формата за регистрация.
    public class RegisterViewModel
    {
        [Required, Display(Name = "Пълно име")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Имейл")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Парола")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password), Display(Name = "Потвърди парола")]
        [Compare(nameof(Password), ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Дали потребителят иска да е организатор на събития.
        [Display(Name = "Искам да организирам събития")]
        public bool RegisterAsOrganizer { get; set; }
    }

    // Данни за формата за вход.
    public class LoginViewModel
    {
        [Required, EmailAddress, Display(Name = "Имейл")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }
    }
}
