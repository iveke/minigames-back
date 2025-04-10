using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;

namespace MiniGame.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public Game(string title)
        {
            this.Title = title;
        }
    }
}
