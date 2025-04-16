using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    public async Task SendEmailAsync(string email, string code)
    {
        string smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
        string password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MiniGames App", smtpEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Підтвердження пошти";

        message.Body = new TextPart("plain")
        {
            Text = $"Ваш код підтвердження: {code}"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, false);
        await client.AuthenticateAsync(smtpEmail, password); // не пароль Gmail, а спец. пароль для додатків
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }



}
