using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class League
    {
        public List<Team_League> Table
        {get; private set;} = new List<Team_League>();

        public List<string> History
        {get; private set;} = new List<string>();
        
        public int Round
        {get; private set;} = 0;

        public bool RegisterTeams(List<Team> teams, bool isCBF)
        {
            // Só pode registrar uma vez com um número par de times de pelo menos 8
            if (!isCBF) {return false;}
            if (teams.Count < 8) {return false;}
            if (teams.Count % 2 != 0) {return false;}
            if (Table.Count > 0) {return false;}

            // Cria um Team_League pra cada Team
            Table = teams.Select(x => new Team_League(x)).ToList();
            return true;
        }

        public bool AddPlayer(string playerName, Team_League team, bool isCBF)
        {
            if (!isCBF) {return false;}
            if (!Table.Contains(team)) {return false;}
            if (team.Players.Count >= 32) {return false;}
            
            // Cria um Player_Team pra cada Player
            team.Players.Add(new Player_Team(playerName, team));
            return true;
        }

        public bool RemovePlayer(string playerName, Team_League team, bool isCBF)
        {
            if (!isCBF) {return false;}
            if (!Table.Contains(team)) {return false;}
            if (team.Players.Count <= 16) {return false;}

            var foundPlayer = team.Players.Find(x => x.Name == playerName);
            return team.Players.Remove(foundPlayer);
        }

        public bool GenerateRound()
        {
            // Prepara a próxima rodada, dando um oponente para cada time
            
            if (Table.Count < 8) {return false;}
            // Quando alcançar o limite de rodadas, não permite mais rodadas
            if (Round == Table.Count*2) {return false;}
            // Garante que todos os times ja jogaram a rodada passada (ou que é a primeira)
            if (Round != 0 && !Table.All(x => x.HasPlayed)) {return false;}

            // Reseta a rodada, deixando os times sem oponente e sem ter jogado ainda
            Table.ForEach(x => {x.HasPlayed = false; x.CurrentOpponent = null;});

            foreach (var team in Table)
            {
                if (team.CurrentOpponent != null) {continue;}

                var random = new Random();
                while (true)
                {
                    // Procura um oponente válido
                    var availableOpponents = Table.Where(x => x != team && x.CurrentOpponent == null).ToList();
                    var opponent = availableOpponents[random.Next(0, availableOpponents.Count)];

                    team.CurrentOpponent = opponent;
                    opponent.CurrentOpponent = team;
                    break;
                }
            }
            Round++;
            return true;
        }

        public List<string> PlayRound(bool isCBF)
        {
            // Joga as partidas definidas pelo GenerateRound(),
            // gerando os resultados e os retornando.
            if (Table.Count < 8) {return null;}
            if (!isCBF) {return null;}

            // Garante que todos os times foram atribuídos
            // um oponente para a rodada
            if (!Table.All(x => x.CurrentOpponent != null)) {return null;}

            var results = new List<string>();
            
            foreach (var team in Table)
            {
                if (team.HasPlayed) {continue;}

                var opponent = team.CurrentOpponent;
                var random = new Random();
                
                // Gera um placar aleatório de 0 a 4 gols
                // e distribui as estatísticas devidamente entre os times
                var score1 = random.Next(0, 5);
                var score2 = random.Next(0, 5);

                team.GoalsFor += score1;
                opponent.GoalsFor += score2;
                team.GoalsAgainst += score2;
                opponent.GoalsAgainst += score1;

                if(score1 > score2)
                {
                    team.Wins++;
                }
                else if(score2 > score1)
                {
                    opponent.Wins++;
                }
                else
                {
                    team.Draws++;
                    opponent.Draws++;
                }

                // Distribui os gols do times entre seus jogadores
                for(var i = 1; i <= score1; i++)
                {
                    var playerIndex = random.Next(0, team.Players.Count);
                    team.Players[playerIndex].GoalsForTeam++;
                }

                for(var i = 1; i <= score2; i++)
                {
                    var playerIndex = random.Next(0, opponent.Players.Count);
                    opponent.Players[playerIndex].GoalsForTeam++;
                }

                team.HasPlayed = true;
                opponent.HasPlayed = true;
                team.PreviousOpponents.Add(opponent);
                opponent.PreviousOpponents.Add(team);

                results.Add($"{team.TeamName} {score1} x {score2} {opponent.TeamName}");
            }

            History.AddRange(results);
            return results;

        }

        public List<string> GetTable()
        {
            if (Table.Count < 8) {return null;}
            if (Round == 0) {return null;}
            
            var result = new List<string>();
           
            // Calcula as estatísticas da tabela com base nas propriedades
            foreach (var team in Table)
            {
                double points = (team.Wins*3) + (team.Draws);
                double played = team.HasPlayed ? Round : Round - 1;
                double defeats = played - team.Wins - team.Draws;
                double diff = team.GoalsFor - team.GoalsAgainst;
                double percentage = played == 0 ? 0 : (points/(played*3)) * 100;
                var resultString = $"{team.TeamName} | {points} | {played} | {team.Wins} | {team.Draws} | {defeats} | {diff} | {team.GoalsFor} | {team.GoalsAgainst} | {percentage.ToString("##0.##")}%";
            
                result.Add(resultString);
            }
            return result;
        }

        public List<string> GetTopGoalscorers()
        {
            var allPlayers = new List<Player_Team>();

            foreach (var team in Table)
            {
                allPlayers.AddRange(team.Players);
            }

            var top10 = allPlayers.OrderByDescending(x => x.GoalsForTeam).Take(10);

            var result = new List<string>();
            foreach (var player in top10)
            {
                result.Add($"{player.GoalsForTeam} - {player.Name} {player.CurrentTeam.TeamName.ToUpper()}");
            }

            return result;
        }

        public List<Team_League> GetLibertadores()
        {
            return Table.OrderByDescending(x => (x.Wins*3) + (x.Draws)).Take(4).ToList();
        }

        public List<Team_League> GetDemoted()
        {
            return Table.OrderBy(x => (x.Wins*3) + (x.Draws)).Take(4).ToList();
        }

        public List<string> GetAllResults()
        {
            return History;
        }
    }
}