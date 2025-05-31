using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using MiniGame.Services; // Додайте цей using

namespace MiniGame.Services // Переконайтеся, що ваш EmailService знаходиться в цьому namespace
{
    public class EmailService // Додайте реалізацію інтерфейсу
    {
        public async Task SendEmailAsync(string email, string code)
        {
            string smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            string password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            // Ці перевірки допоможуть уникнути ArgumentNullException в реальному застосунку
            if (string.IsNullOrEmpty(smtpEmail))
            {
                throw new InvalidOperationException("SMTP_EMAIL environment variable is not set.");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("SMTP_PASSWORD environment variable is not set.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MiniGames App", smtpEmail));
            message.To.Add(new MailboxAddress(email, email)); // Краще передавати email як ім'я та адресу
            message.Subject = "Підтвердження пошти";

            message.Body = new TextPart("plain")
            {
                Text = $"Ваш код підтвердження: {code}"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false); // Можете використовувати SecureSocketOptions.StartTls
                await client.AuthenticateAsync(smtpEmail, password);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Якщо виникла помилка під час відправлення пошти, перевикиньте її.
                // Це призведе до провалу тесту, якщо він використовує реальний сервіс,
                // або дозволить моку імітувати помилку.
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}