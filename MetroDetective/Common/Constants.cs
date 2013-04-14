using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroDetective.Common
{
    public class Constants
    {
        public const int HotSpotToleranceRadius = 20;
        public const string LevelCompleteMsg = @"You have completed this level and won 1 bonus hint point.";
        public const string LevelCompleteTitle = @"Level Completed!";

        public const string GameCompleteMsg = "Congratulations, you have completed all levels.";
        public const string GameCompleteTitle = @"Well Done!!!";

        public const string HelpMsg =
            "\n" + "1) Click on the either screen to spot the differences." + "\n" + "\n" +
            "2) You need to find all 5 differences to clear a level." + "\n" + "\n" +
            "3) click hint to show the next difference.";

        public const int HotSpotNumbers = 5;
    }
}
