﻿using System.Collections.Generic;

namespace PPCounter.Utilities
{

    static class Structs
    {
        public class SongID
        {
            public string id;
            public BeatmapDifficulty difficulty;

            public SongID(string id, BeatmapDifficulty difficulty)
            {
                this.id = id;
                this.difficulty = difficulty;
            }

            public override bool Equals(System.Object obj)
            {
                return this.id == ((SongID) obj).id && this.difficulty == ((SongID) obj).difficulty;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode() + difficulty.GetHashCode();
            }
        }

        public class RawPPData
        {
            public float _Easy_SoloStandard { get; set; }
            public float _Normal_SoloStandard { get; set; }
            public float _Hard_SoloStandard { get; set; }
            public float _Expert_SoloStandard { get; set; }
            public float _ExpertPlus_SoloStandard { get; set; }
        }

        public class AccSaberRankedMap
        {
            public string difficulty;
            public string songHash;
            public float complexity;
        }

        public class Leaderboards
        {
            public ScoreSaber ScoreSaber { get; set; }
            public BeatLeader BeatLeader { get; set; }
            public AccSaber AccSaber { get; set; }
        }

        public class ScoreSaber
        {
            public List<Point> modifierCurve { get; set; }
            public List<Point> standardCurve { get; set; }
            public List<string> songsAllowingPositiveModifiers { get; set; }
            public ScoreSaberModifiers modifiers { get; set; }
        }

        public class ScoreSaberModifiers
        {
            public float da;
            public float gn;
            public float fs;
        }

        public class BeatLeader
        {
            public List<Point> accCurve { get; set; }
            public float accMultiplier { get; set; }
            public float passExponential { get; set; }
            public float passMultiplier { get; set; }
            public float passShift { get; set; }

            public float techExponentialMultiplier { get; set; }
            public float techMultiplier { get; set; }

            public float inflateExponential { get; set; }
            public float inflateMultiplier { get; set; }

        }

        public class AccSaber
        {
            public List<Point> curve { get; set; }
            public float scale;
            public float shift;
        }

        public class Point
        {
            public float x { get; set; }
            public float y { get; set; }
        }
    }
}
