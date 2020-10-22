﻿using System;
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
            team.Players.Add(new Player_Team(playerName));
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

        public void GenerateRound()
        {
            // Prepara a próxima rodada, dando um oponente para cada time

            // Garante que todos os times ja jogaram a rodada passada (ou que é a primeira)
            if (Round != 0 && !Table.All(x => x.HasPlayed)) {return;}

            // Reseta a rodada, deixando os times sem oponente e sem ter jogado ainda
            Table.Select(x => x.CurrentOpponent = null).Select(x => x.HasPlayed = false);
            
            foreach (var team in Table)
            {
                if (team.CurrentOpponent != null) {continue;}
                
                var random = new Random();

                while (true)
                {
                    var opponent = Table[random.Next(0, Table.Count)];

                    // Procura um oponente válido
                    if (opponent == team) {continue;}
                    if (opponent.CurrentOpponent != null) {continue;}
                    if (team.PreviousOpponents.Contains(opponent)) {continue;}

                    team.CurrentOpponent = opponent;
                    opponent.CurrentOpponent = team;
                    break;
                }
            }

            Round++;

        }

        public List<string> PlayRound(bool isCBF)
        {
            // Joga as partidas definidas pelo GenerateRound(),
            // gerando os resultados e os retornando.
            
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
                var score1 = random.Next(0, 4);
                var score2 = random.Next(0, 4);

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
                    var playerIndex = random.Next(0, team.Players.Count - 1);
                    team.Players[playerIndex].GoalsForTeam++;
                }

                for(var i = 1; i <= score2; i++)
                {
                    var playerIndex = random.Next(0, opponent.Players.Count - 1);
                    opponent.Players[playerIndex].GoalsForTeam++;
                }

                team.HasPlayed = true;
                opponent.HasPlayed = true;
                team.PreviousOpponents.Add(opponent);
                opponent.PreviousOpponents.Add(team);

                results.Add($"{team.TeamName} {score1} x {score2} {opponent.TeamName}");
            }

            return results;

        }

       public List<string> GetTable()
       {
           var result = new List<string>();
           
           // Calcula as estatísticas da tabela com base nas propriedades
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
