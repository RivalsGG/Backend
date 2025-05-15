using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RivalsGG.Core.Models
{
    public class PlayerProfile
    {
        public long Uid { get; set; }
        public Player Player { get; set; } = new Player();
    }
}
