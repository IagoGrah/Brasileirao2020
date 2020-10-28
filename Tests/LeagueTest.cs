using Domain;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class LeagueTest
    {
        public List<Player> GetMockPlayers(int amount)
        {
            var MockPlayers = new List<Player>
            {
                new Player("Jonas"), new Player("Rubens"), new Player("Nelson"), new Player("Ramos"),
                new Player("Kelvin"), new Player("Fabinho"), new Player("Luka"), new Player("Kaká"),
                new Player("Luizinho"), new Player("Marcão"), new Player("Roni"), new Player("Naldo"),
                new Player("Kennedy"), new Player("Leonardo"), new Player("Muralha"), new Player("Pedrão"),
                new Player("Assis"), new Player("Anão"), new Player("José"), new Player("Fernando"),
                new Player("Tulio"), new Player("César"), new Player("Miranda"), new Player("Cris"),
                new Player("Wesley"), new Player("Jadson"), new Player("Nicolau"), new Player("Ney"),
                new Player("Messias"), new Player("Vladimir"), new Player("Wagner"), new Player("Raul")
            };

            return MockPlayers.Take(amount).ToList();
        }
        
        public List<Team> GetMockTeams(int amount)
        {
            var MockTeams = new List<Team>
            {
                new Team("Flasco", GetMockPlayers(24)), new Team("Vortex", GetMockPlayers(24)),
                new Team("Linux", GetMockPlayers(24)), new Team("Windows", GetMockPlayers(24)),
                new Team("Foxtrot", GetMockPlayers(24)), new Team("Blumenau", GetMockPlayers(24)),
                new Team("Bahia", GetMockPlayers(24)), new Team("Mengão", GetMockPlayers(24)),
                new Team("Barcelona", GetMockPlayers(24)), new Team("Galo", GetMockPlayers(24)),
                new Team("Espartanos", GetMockPlayers(24)), new Team("Candidatos", GetMockPlayers(24)),
                new Team("Atléticos", GetMockPlayers(24)), new Team("Esportistas", GetMockPlayers(24)),
                new Team("Brabos", GetMockPlayers(24)), new Team("Goleadores", GetMockPlayers(24))
            };

            return MockTeams.Take(amount).ToList();
        }
        
        [Fact]
        public void should_return_false_and_not_register_when_less_than_8_teams()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(7);

            var result = bras.RegisterTeams(teamsInput, true);

            Assert.False(result);
            Assert.Empty(bras.Table);
        }

        [Fact]
        public void should_return_false_and_not_register_when_not_CBF()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(8);

            var result = bras.RegisterTeams(teamsInput, false);

            Assert.False(result);
            Assert.Empty(bras.Table);
        }
        
        [Fact]
        public void should_return_false_and_not_register_when_odd_number_of_teams()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(11);

            var result = bras.RegisterTeams(teamsInput, true);

            Assert.False(result);
            Assert.Empty(bras.Table);
        }

        [Fact]
        public void should_return_false_and_not_register_when_invalid_squad_size()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(7);
            teamsInput.Add(new Team("Precarios", GetMockPlayers(15)));

            var result = bras.RegisterTeams(teamsInput, true);

            Assert.False(result);
            Assert.Empty(bras.Table);
        }

        [Fact]
        public void should_return_false_and_not_register_the_second_time()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(8);
            var teamsInput2 = GetMockTeams(8);
            bras.RegisterTeams(teamsInput, true);

            var result = bras.RegisterTeams(teamsInput2, true);
            
            Assert.False(result);
            Assert.Equal(8, bras.Table.Count);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_return_true_and_register_teams(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);

            var result = bras.RegisterTeams(teamsInput, true);

            Assert.True(result);
            Assert.Equal(size, bras.Table.Count);
        }

        [Fact]
        public void should_return_false_and_not_add_when_not_CBF()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[0];
            var result = bras.AddPlayer("Mauricio", team, false);

            Assert.False(result);
            Assert.False(team.Players.Exists(x => x.Name == "Mauricio"));
        }

        [Fact]
        public void should_not_add_when_team_doesnt_exist()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = new Team_League(new Team("Inexistentes", GetMockPlayers(24)));
            var result = bras.AddPlayer("Mauricio", team, true);

            Assert.False(result);
            Assert.False(team.Players.Exists(x => x.Name == "Mauricio"));
        }
        
        [Fact]
        public void should_not_add_when_team_is_full()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(9);
            teamsInput.Add(new Team("Cheios", GetMockPlayers(32)));
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[9];
            var result = bras.AddPlayer("Mauricio", team, true);

            Assert.False(result);
            Assert.False(team.Players.Exists(x => x.Name == "Mauricio"));
        }
        
        [Theory]
        [InlineData("Ramalho")]
        [InlineData("Zé")]
        [InlineData("Gypsy")]
        [InlineData("Lala")]
        public void should_return_true_and_add_player(string pName)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[0];
            var result = bras.AddPlayer(pName, team, true);

            Assert.True(result);
            Assert.True(team.Players.Exists(x => x.Name == pName));
        }

        [Fact]
        public void should_return_false_and_not_remove_when_not_CBF()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[0];
            var result = bras.RemovePlayer("Jonas", team, false);

            Assert.False(result);
            Assert.True(team.Players.Exists(x => x.Name == "Jonas"));
        }

        [Fact]
        public void should_not_remove_when_team_doesnt_exist()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = new Team_League(new Team("Inexistentes", GetMockPlayers(24)));
            var result = bras.RemovePlayer("Jonas", team, true);

            Assert.False(result);
            Assert.True(team.Players.Exists(x => x.Name == "Jonas"));
        }

        [Fact]
        public void should_not_remove_when_team_has_16()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(9);
            teamsInput.Add(new Team("Vazios", GetMockPlayers(16)));
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[9];
            var result = bras.RemovePlayer("Jonas", team, true);

            Assert.False(result);
            Assert.True(team.Players.Exists(x => x.Name == "Jonas"));
        }

        [Theory]
        [InlineData("Jonas")]
        [InlineData("Rubens")]
        [InlineData("Nelson")]
        [InlineData("Ramos")]
        public void should_return_true_and_remove_player(string pName)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(10);
            bras.RegisterTeams(teamsInput, true);

            var team = bras.Table[0];
            var result = bras.RemovePlayer(pName, team, true);

            Assert.True(result);
            Assert.False(team.Players.Exists(x => x.Name == pName));
        }

        [Fact]
        public void should_return_false_and_not_generate_round()
        {
            var bras = new League();

            var result = bras.GenerateRound();

            Assert.False(result);
            Assert.Equal(0, bras.Round);
        }

        [Fact]
        public void should_not_generate_round_before_played()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(8);
            bras.RegisterTeams(teamsInput, true);
            bras.GenerateRound();
            
            var result = bras.GenerateRound();

            Assert.False(result);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_generate_round(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);

            var result = bras.GenerateRound();

            Assert.True(result);
            Assert.True(bras.Table.All(x => x.CurrentOpponent != null));
            Assert.Equal(1, bras.Round);
        }

        [Fact]
        public void should_return_null_when_not_cbf()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(8);
            bras.RegisterTeams(teamsInput, true);
            bras.GenerateRound();

            var result = bras.PlayRound(false);

            Assert.Null(result);
        }

        [Fact]
        public void should_return_null_when_no_teams()
        {
            var bras = new League();
            
            var result = bras.PlayRound(true);

            Assert.Null(result);
        }
        
        [Fact]
        public void should_return_null_when_round_not_generated()
        {
            var bras = new League();
            var teamsInput = GetMockTeams(8);
            bras.RegisterTeams(teamsInput, true);

            var result = bras.PlayRound(true);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_play_round(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            bras.GenerateRound();

            var result = bras.PlayRound(true);

            Assert.NotNull(result);
            Assert.Equal(size/2, result.Count);
            Assert.True(bras.Table.All(x => x.HasPlayed));
            Assert.True(bras.Table.All(x => x.GoalsFor >= 0 && x.GoalsFor <= 4));
            Assert.False(bras.Table.All(x => x.Points == 0));
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_generate_round_after_playing(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            bras.GenerateRound();
            bras.PlayRound(true);

            var result = bras.GenerateRound();

            Assert.True(result);
            Assert.True(bras.Table.All(x => x.CurrentOpponent != null));
            Assert.Equal(2, bras.Round);
        }
        
        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_play_second_round(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            bras.GenerateRound();
            bras.PlayRound(true);
            bras.GenerateRound();

            var result = bras.PlayRound(true);

            Assert.NotNull(result);
            Assert.Equal(size/2, result.Count);
            Assert.True(bras.Table.All(x => x.HasPlayed));
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(16)]
        public void should_not_generate_round_if_reaches_limit(int size)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < size*2; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GenerateRound();

            Assert.False(result);
            Assert.Equal(size*2, bras.Round);
        }
        
        [Fact]
        public void should_return_null_if_there_are_no_teams()
        {
            var bras = new League();

            var result = bras.GetTable();

            Assert.Null(result);
        }

        [Theory]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(12, 12)]
        [InlineData(14, 22)]
        [InlineData(16, 32)]
        public void should_return_table(int size, int rounds)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < rounds; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GetTable();

            Assert.NotNull(result);
            Assert.Equal(size, result.Count);
        }

        [Fact]
        public void should_return_null_when_there_are_no_teams()
        {
            var bras = new League();

            var result = bras.GetTopGoalscorers();

            Assert.Null(result);
        }
        
        [Theory]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(12, 12)]
        [InlineData(14, 22)]
        [InlineData(16, 32)]
        public void should_return_top_10_goalscorers(int size, int rounds)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < rounds; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GetTopGoalscorers();

            Assert.NotNull(result);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void should_all_return_null_if_there_are_no_teams()
        {
            var bras = new League();

            var result1 = bras.GetLibertadores();
            var result2 = bras.GetDemoted();
            var result3 = bras.GetAllResults();

            Assert.Null(result1);
            Assert.Null(result2);
            Assert.Null(result3);
        }
        
        [Theory]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(12, 12)]
        [InlineData(14, 22)]
        [InlineData(16, 32)]
        public void should_return_top_4_teams(int size, int rounds)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < rounds; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GetLibertadores();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }

        [Theory]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(12, 12)]
        [InlineData(14, 22)]
        [InlineData(16, 32)]
        public void should_return_bottom_4_teams(int size, int rounds)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < rounds; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GetDemoted();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }

        [Theory]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(12, 12)]
        [InlineData(14, 22)]
        [InlineData(16, 32)]
        public void should_return_round_history(int size, int rounds)
        {
            var bras = new League();
            var teamsInput = GetMockTeams(size);
            bras.RegisterTeams(teamsInput, true);
            
            for (int i = 0; i < rounds; i++)
            {
                bras.GenerateRound();
                bras.PlayRound(true);
            }

            var result = bras.GetAllResults();

            Assert.NotNull(result);
            Assert.Equal(rounds*size/2, result.Count);
        }
    }
}
