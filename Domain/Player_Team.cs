namespace Domain
{
    public class Player_Team : Player
    {
        public Team CurrentTeam
        {get; private set;}
        
        public int GoalsForTeam
        {get; set;} = 0;

        public Player_Team(Player player, Team team) : base(player.Name)
        {
            CurrentTeam = team;
        }

        public Player_Team(string name, Team team) : base(name)
        {
            CurrentTeam = team;
        }
    }
}
