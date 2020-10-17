using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Player_Team : Player
    {
        public int GoalsForTeam
        {get; set;} = 0;

        public Player_Team(Player player) : base(player.Name)
        {}

        public Player_Team(string name) : base(name)
        {}
    }
}
