using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;

namespace MiniGame.Models
{
    public class Result
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public virtual Game Game { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public DateTime PlayTime { get; set; }
        [Required]
        public int Points { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public int Level { get; set; }

        public Result() { }
        public Result(int GameId, int UserId, DateTime PlayTime, int Points, int Duration, int Level)
        {
            this.GameId = GameId;
            this.UserId = UserId;
            this.PlayTime = PlayTime;
            this.Points = Points;
            this.Duration = Duration;
            this.Level = Level;
        }
    }
}
