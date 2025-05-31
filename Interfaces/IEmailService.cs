using System.Threading.Tasks;

namespace MiniGame.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string code);
    }
}