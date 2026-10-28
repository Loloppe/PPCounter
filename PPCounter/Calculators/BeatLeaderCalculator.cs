using Newtonsoft.Json;
using PPCounter.Data;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Calculators
{
    internal class BeatLeaderCalculator
    {
        private string _leaderboardId;

        private List<Point> _ppCurve;
        private float[] _ppSlopes;
        private float _stars;

        public void SetCurve(GameplayModifiers modifiers)
        {
            var codes = new List<string>();

            if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster)) codes.Add("FS");
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.SuperFast)) codes.Add("SF");
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Slower)) codes.Add("SS");
            if (modifiers.disappearingArrows) codes.Add("DA");
            if (modifiers.ghostNotes) codes.Add("GN");
            if (modifiers.noArrows) codes.Add("NA");
            if (modifiers.noBombs) codes.Add("NB");
            if (modifiers.enabledObstacleType.Equals(GameplayModifiers.EnabledObstacleType.NoObstacles)) codes.Add("NO");
            if (modifiers.proMode) codes.Add("PM");
            if (modifiers.smallCubes) codes.Add("SC");

            LoadCurve(codes);
        }

        public void SetCurve(List<string> modifiers)
        {
            var codes = new List<string>();

            if (modifiers.Contains("Fast")) codes.Add("FS");
            else if (modifiers.Contains("Super Fast")) codes.Add("SF");
            else if (modifiers.Contains("Slower")) codes.Add("SS");

            LoadCurve(codes);
        }

        private void LoadCurve(List<string> modifiers)
        {
            _ppCurve = null;
            _ppSlopes = null;
            _stars = 0;

            var curve = GetCurve(modifiers).ConfigureAwait(false).GetAwaiter().GetResult();
            if (curve?.points == null || curve.points.Count < 2)
            {
                return;
            }

            _ppCurve = curve.points.Select(p => new Point { x = p[0], y = p[1] }).OrderBy(p => p.x).ToList();
            _ppSlopes = CurveUtils.GetSlopes(_ppCurve);
            _stars = curve.stars;
        }

        private async Task<BLCurve> GetCurve(List<string> modifiers)
        {
            try
            {
                string url = "https://api.beatleader.com/pp/curve?leaderboardId=" + _leaderboardId + "&modifiers=" + string.Join(",", modifiers);
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(url).ConfigureAwait(false);
                using HttpContent content = res.Content;
                var str = await content.ReadAsStringAsync().ConfigureAwait(false);
                if (str != null)
                {
                    BLCurve curve = JsonConvert.DeserializeObject<BLCurve>(str);
                    if (curve != null && curve.hasPp)
                    {
                        return curve;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.log.Warn(e);
                Plugin.log.Warn("Error during BL pp curve fetch from API for this map");
            }

            return null;
        }

        public async Task<bool> GetData(SongID songID)
        {
            try
            {
                _leaderboardId = null;

                string url = "https://api.beatleader.com/leaderboards/hash/" + songID.id;
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(url).ConfigureAwait(false);
                using HttpContent content = res.Content;
                var str = await content.ReadAsStringAsync().ConfigureAwait(false);
                if (str != null)
                {
                    BLLeaderboards obj = JsonConvert.DeserializeObject<BLLeaderboards>(str);
                    BLLeaderboard leaderboard = obj?.leaderboards?.FirstOrDefault(l => l.difficulty?.difficultyName == songID.difficulty.ToString());
                    if (leaderboard?.difficulty.status == 3) // Ranked
                    {
                        _leaderboardId = leaderboard.id;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.log.Warn(e);
                Plugin.log.Warn("Error during BL data fetch from API for this map");
            }

            return false;
        }

        public float GetStars()
        {
            return _stars;
        }

        public float CalculatePP(float accuracy)
        {
            if (_ppCurve == null)
            {
                return 0;
            }

            float pp = CurveUtils.GetCurveMultiplier(_ppCurve, _ppSlopes, accuracy);
            if (float.IsInfinity(pp) || float.IsNaN(pp))
            {
                pp = 0;
            }

            return pp;
        }
    }
}
