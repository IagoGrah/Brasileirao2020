using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Team
    {
        public string TeamName
        {get; protected set;}

        public List<Player_Team> Players
        {get; set;}

        public Team(string name, List<Player> players)
        {
            TeamName = name;
            Players = players.Select(x => new Player_Team(x)).ToList();
        }

        protected Team(string name, List<Player_Team> players)
        {
            TeamName = name;
            Players = players;
        }
    }
}
