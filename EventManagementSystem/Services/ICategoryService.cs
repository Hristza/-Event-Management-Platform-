using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    // "Интерфейс" = списък с обещания: какво МОЖЕ да прави сервизът,
    // но не и КАК. Контролерите зависят от интерфейса, не от класа.
    // Това позволява в тестовете да подменим (mock) реалния клас.
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
