using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Core.Models
{
    public class HeroPerformance
    {
        public string HeroId { get; set; } = string.Empty;
        public string HeroName { get; set; } = string.Empty;
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public double WinRate { get; set; }
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
        public int Eliminations { get; set; }
    }
}
