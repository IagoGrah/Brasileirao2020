using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Team
    {
        public string Name
        {get; private set;}

        public List<Player> Players
        {get; set;}
    }
}
