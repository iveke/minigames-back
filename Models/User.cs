using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;

namespace MiniGame.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        [Required]
        public string Email { get; set; }
        public string Country { get; set; }
        public float AvgTotalScore = 0;
        public string Phone{ get; set; }
        [Required]
        public RoleEnum Role = RoleEnum.USER;
        public bool ConfirmEmail { get; set; } = false;

        public User(string email, string username, string password, string? phone = null)
        {
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.Phone = phone;
        }

        // Метод для хешування пароля за допомогою bcrypt
        public static string HashPassword(string password)
        {
            // Генеруємо солю та хешуємо пароль
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Метод для перевірки пароля за допомогою bcrypt
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Перевіряємо, чи збігається введений пароль з хешованим
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
