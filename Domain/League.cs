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

        public List<string> PlayRound(bool isCBF)
        {
            if (!isCBF) {return null;}

            if (!Table.All(x => x.CurrentOpponent != null)) {return null;}

            var results = new List<string>();
            
            foreach (var team in Table)
            {
                if (team.HasPlayed) {continue;}

                var random = new Random();
                var score1 = random.Next(0, 4);
                var score2 = random.Next(0, 4);

                team.GoalsFor += score1;
                team.CurrentOpponent.GoalsFor += score2;
                team.GoalsAgainst += score2;
                team.CurrentOpponent.GoalsAgainst += score1;

                if(score1 > score2)
                {
                    team.Wins++;
                }
                else if(score2 > score1)
                {
                    team.CurrentOpponent.Wins++;
                }
                else
                {
                    team.Draws++;
                    team.CurrentOpponent.Draws++;
                }

                for(var i = 1; i <= score1; i++)
                {
                    var playerIndex = random.Next(0, team.Players.Count - 1);
                    team.Players[playerIndex].GoalsForTeam++;
                }

                for(var i = 1; i <= score2; i++)
                {
                    var playerIndex = random.Next(0, team.CurrentOpponent.Players.Count - 1);
                    team.CurrentOpponent.Players[playerIndex].GoalsForTeam++;
                }

                team.HasPlayed = true;
                team.CurrentOpponent.HasPlayed = true;
                team.PreviousOpponents.Add(team.CurrentOpponent);
                team.CurrentOpponent.PreviousOpponents.Add(team);

                results.Add($"{team.TeamName} {score1} x {score2} {team.CurrentOpponent.TeamName}");
            }

            return results;

        }

       public List<string> GetTable()
       {
           var result = new List<string>();
           
           foreach (var team in Table)
           {
               double points = (team.Wins*3) + (team.Draws);
               double played = team.HasPlayed ? Round : Round - 1;
               double defeats = played - team.Wins - team.Draws;
               double diff = team.GoalsFor - team.GoalsAgainst;
               double percentage = played == 0 ? 0 : (points/(played*3)) * 100;
               var resultString = $"{team.TeamName} - {points} - {played} - {team.Wins} - {team.Draws} - {defeats} - {diff} - {team.GoalsFor} - {team.GoalsAgainst} - {percentage}";
               
               result.Add(resultString);
           }
           return result;
       }
    }
}
