using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class League
    {
        public List<Team_League> Table
        {get; private set;}

        public int Round
        {get; set;}

        public bool RegisterTeams(List<Team> teams, bool isCBF)
        {
            if (!isCBF) {return false;}
            if (teams.Count < 8) {return false;}
            if (teams.Count % 2 != 0) {return false;}
            if (Table.Count > 0) {return false;}

            Table = teams.Select(x => new Team_League(x)).ToList();
            return true;
        }

        public bool AddPlayer(string playerName, Team_League team, bool isCBF)
        {
            if (!isCBF) {return false;}
            if (team.Players.Count >= 32) {return false;}
            
            team.Players.Add(new Player_Team(playerName));
            return true;
        }

        public bool RemovePlayer(string playerName, Team_League team, bool isCBF)
        {
            if (!isCBF) {return false;}
            if (team.Players.Count <= 16) {return false;}

            var foundPlayer = team.Players.Find(x => x.Name == playerName);
            return team.Players.Remove(foundPlayer);
        }

        public void GenerateRound()
        {
            if (Round != 0 && !Table.All(x => x.HasPlayed)) {return;}

            Table.Select(x => x.CurrentOpponent = null).Select(x => x.HasPlayed = false);
            
            foreach (var team in Table)
            {
                if (team.CurrentOpponent != null) {continue;}
                
                var random = new Random();

                while (true)
                {
                    var opponent = Table[random.Next(0, Table.Count)];

                    if (opponent == team) {continue;}
                    if (opponent.CurrentOpponent != null) {continue;}
                    if (team.PreviousOpponents.Contains(opponent)) {continue;}

                    team.CurrentOpponent = opponent;
                    team.PreviousOpponents.Add(opponent);
                    opponent.CurrentOpponent = team;
                    opponent.PreviousOpponents.Add(team);
                    break;
                }
            }

            Round++;
        }

        public bool PlayRound(bool isCBF)
        {
            if (!isCBF) {return false;}

            if (!Table.All(x => x.CurrentOpponent != null)) {return false;}
            
            foreach (var team in Table)
            {
                // Play round
            }

            return true;
        }
    }
}
