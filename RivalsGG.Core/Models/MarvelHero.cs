using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RivalsGG.Core.Models
{
    public class MarvelHero
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Real_Name { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Lore { get; set; } = string.Empty;
        public string[] Team { get; set; } = Array.Empty<string>();
        public MarvelHeroAbilities[] Abilities { get; set; } = Array.Empty<MarvelHeroAbilities>();
    }
}
