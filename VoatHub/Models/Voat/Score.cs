using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.Voat
{
    public class Score
    {
        public int sum { get; set; }
        public int upCount { get; set; }
        public int downCount { get; set; }

        public override string ToString()
        {
            return string.Format("Total: {0} Up: {1} Down: {2}", sum, upCount, downCount);
        }
    }
}
