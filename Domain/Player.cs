using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Player
    {
        public string Name
        {get; protected set;}

        public Player(string name)
        {
            Name = name;
        }
    }
}
