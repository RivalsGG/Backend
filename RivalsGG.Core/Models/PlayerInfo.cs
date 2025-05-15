using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RivalsGG.Core.Models
{
    public class PlayerInfo
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalMatches { get; set; }
        public double WinRate { get; set; }
        public int Experience { get; set; }
        public List<HeroPerformance> HeroStats { get; set; } = new List<HeroPerformance>();
    }
}
