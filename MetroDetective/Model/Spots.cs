using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroDetective.Model
{
    public class HotSpot
    {
        public double X { get; set; }

        public double Y { get; set; }

        public bool Checked { get; set; }
    }

    public class PaintingHotSpots
    {
        public List<HotSpot> Spots { get; set; }
        public string PaintingName { get; set; }
    }

    public class GameHotSpots
    {
        public List<PaintingHotSpots> PaintingSpots { get; set; }
    }
}
