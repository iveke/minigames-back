using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MyBackend.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Age = u.Age,
                    Email = u.Email,
                    Country = u.Country,
                    AvgTotalScore = u.AvgTotalScore,
                    Phone = u.Phone,
                    Role = u.Role,
                    ConfirmEmail = u.ConfirmEmail
                })
                .ToListAsync();
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

        public async Task<User> UpdateUserAsync(int userId, UpdateModel payload)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return user;

            // Перевіряємо унікальність username
            if (!string.IsNullOrEmpty(payload.username) && payload.username != user.Username)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == payload.username);
                if (existingUser != null) return user; // Username вже зайнятий
                user.Username = payload.username;
            }

            if (!string.IsNullOrEmpty(payload.phone))
                user.Phone = payload.phone;

            if (payload.Age.HasValue)
                user.Age = payload.Age.Value;

            if (!string.IsNullOrEmpty(payload.Country))
                user.Country = payload.Country;

            await _context.SaveChangesAsync();
            return user;
        }

        // Підтвердження email
        public async Task<bool> ConfirmEmailAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false; // Користувач не знайдений

            if (user.ConfirmEmail)
            {
                throw new InvalidOperationException("Email is already confirmed.");
            }

            user.ConfirmEmail = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // Видалити користувача

        public async Task<bool> DeleteUserAsync(int id, string role)
        {
            // Перевіряємо, чи користувач має права видаляти
            if (role != "ADMIN") throw new UnauthorizedAccessException("Access denied");

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

        // [Required(ErrorMessage = "Phone number is required")]
        // public string phone { get; set; }

    }
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string? Country { get; set; }
        public float? AvgTotalScore;
        public string? Phone { get; set; }
        public RoleEnum Role;
        public bool ConfirmEmail { get; set; }
    }

}
