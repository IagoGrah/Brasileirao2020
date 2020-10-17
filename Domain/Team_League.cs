using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Team_League : Team
    {
        public int Wins
        {get; set;} = 0;

        public int Draws
        {get; set;} = 0;

        public int GoalsFor
        {get; set;} = 0;

        public int GoalsAgainst
        {get; set;} = 0;

        public Team_League CurrentOpponent
        {get; set;} = null;

        public List<Team_League> PreviousOpponents
        {get; set;} = new List<Team_League>();

        public bool HasPlayed
        {get; set;} = false;

        public Team_League(Team team) : base(team.TeamName, team.Players)
        {}
    }
}
