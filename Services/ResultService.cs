using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiniGame.Services
{
    public class ResultService
    {
        private readonly AppDbContext _context;

        public ResultService(AppDbContext context)
        {
            _context = context;
        }

        // Отримати список результатів
        public async Task<IEnumerable<Result>> GetResultsAsync()
        {
            return await _context.Results.ToListAsync();
        }

        // Отримати результат за її ID
        public async Task<Result> GetResultByIdAsync(int id)
        {
            return await _context.Results.FindAsync(id);
        }

        // Збереження результату
        public async Task<Result> CreateResultAsync(SaveResult data)
        {
            Result result = new Result(data.GameId, data.UserId, data.PlayTime, data.Points, data.Duration, data.Level);
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }



        public class SaveResult
        {

            [Required(ErrorMessage = "Game id is requied")]
            public int GameId { get; set; }

            [Required(ErrorMessage = "User id is requied")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "PlayTime is requied")]
            public DateTime PlayTime { get; set; }

            [Required(ErrorMessage = "Points are requied")]
            public int Points { get; set; }

            [Required(ErrorMessage = "Duration is requied")]
            public int Duration { get; set; }

            [Required(ErrorMessage = "Level is requied")]
            public int Level { get; set; }

        }
    }
}
