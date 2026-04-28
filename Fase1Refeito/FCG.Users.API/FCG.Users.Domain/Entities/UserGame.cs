using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Users.Domain.Entities
{
    public class UserGame
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;

        public DateTime AcquiredAt { get; set; }
    }
}
