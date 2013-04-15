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
        public PaintingHotSpots()
        {
            Description = "Painting Description";
            PaintingName = "name.jpg";
            Title = "Titel";
            Artist = "artist";
            Spots = new List<HotSpot>();
        }

        public List<HotSpot> Spots { get; set; }
        public string PaintingName { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
    }

    public class GameHotSpots
    {
        public GameHotSpots()
        {
            PaintingSpots= new List<PaintingHotSpots>();
        }
        public List<PaintingHotSpots> PaintingSpots { get; set; }
    }
}
