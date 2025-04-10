using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiniGame.Services
{
    public class GameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        // Отримати список доступних ігор
        public async Task<IEnumerable<Game>> GetGamesAsync()
        {
            return await _context.Games.ToListAsync();
        }

        // Отримати гру за її ID
        public async Task<Game> GetGameByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        // Створити нову гру
        public async Task<Game> CreateGameAsync(CreateGameModel data)
        {
            Game game = new Game(data.title);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        // Оновити дані гри
        public async Task<bool> UpdateGameAsync(int id, Game game)
        {
            if (id != game.Id) return false;

            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Видалити гру
        public async Task<bool> DeleteGameAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    public class CreateGameModel
    {

        [Required(ErrorMessage = "Game title is requied")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters")]
        public string title { get; set; }

    }
}
