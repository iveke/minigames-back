using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiniGame.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Отримати список користувачів
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Отримати користувача за його ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Створити нового користувача
        public async Task<User> CreateUserAsync(CreateModel data)
        {
            var password = User.HashPassword(data.password);
            User user = new User(data.email, data.username, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Оновити дані користувача
        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            if (id != user.Id) return false;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Видалити користувача
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    public class CreateModel
    {

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string password { get; set; }
    }
}
