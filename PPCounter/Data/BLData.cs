using System.Collections.Generic;

namespace PPCounter.Data
{
    public class BLLeaderboards
    {
        public List<BLLeaderboard> leaderboards { get; set; }
    }

    public class BLLeaderboard
    {
        public string id { get; set; }
        public BLDifficulty difficulty { get; set; }
    }

    public class BLDifficulty
    {
        public string difficultyName { get; set; }
        public string modeName { get; set; }
        public int status { get; set; }
    }

    public class BLCurve
    {
        public string leaderboardId { get; set; }
        public int status { get; set; }
        public bool hasPp { get; set; }
        public string modifiers { get; set; }
        public float stars { get; set; }
        public List<float[]> points { get; set; }
    }
}
