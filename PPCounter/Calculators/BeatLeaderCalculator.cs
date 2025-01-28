using Newtonsoft.Json;
using PPCounter.Data;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Calculators
{
    internal class BeatLeaderRating
    {
        public float Pass { get; set; }
        public float Tech { get; set; }
        public float Acc { get; set; }

        public BeatLeaderRating(float a, float p, float t)
        {
            Pass = p;
            Tech = t;
            Acc = a;
        }
    }

    internal class BeatLeaderCalculator
    {
        private BLData _currentMapData;

        private List<Point> _accCurve;
        private float[] _accSlopes;
        private float _accMultiplier;

        private float _passExponential;
        private float _passMultiplier;
        private float _passShift;

        private float _techExponentialMultiplier;
        private float _techMultiplier;

        private float _inflateExponential;
        private float _inflateMultiplier;

        private float _modifierMultiplier;
        private float _powerBottom;

        BeatLeaderRating _rating;
        private float _passPP;

        public void SetCurve(Structs.BeatLeader beatLeader, GameplayModifiers modifiers)
        {
            _accCurve = beatLeader.accCurve;
            _accMultiplier = beatLeader.accMultiplier;

            _passExponential = beatLeader.passExponential;
            _passMultiplier = beatLeader.passMultiplier;
            _passShift = beatLeader.passShift;

            _techExponentialMultiplier = beatLeader.techExponentialMultiplier;
            _techMultiplier = beatLeader.techMultiplier;

            _inflateExponential = beatLeader.inflateExponential;
            _inflateMultiplier = beatLeader.inflateMultiplier;

            CalculateModifiersMultiplier(modifiers);

            _powerBottom = 0;

            _rating = GetStars(modifiers);
            _passPP = GetPassPP(_rating.Pass);
            _accSlopes = CurveUtils.GetSlopes(_accCurve);
        }

        public void SetCurve(Structs.BeatLeader beatLeader, List<string> modifiers)
        {
            _accCurve = beatLeader.accCurve;
            _accMultiplier = beatLeader.accMultiplier;

            _passExponential = beatLeader.passExponential;
            _passMultiplier = beatLeader.passMultiplier;
            _passShift = beatLeader.passShift;

            _techExponentialMultiplier = beatLeader.techExponentialMultiplier;
            _techMultiplier = beatLeader.techMultiplier;

            _inflateExponential = beatLeader.inflateExponential;
            _inflateMultiplier = beatLeader.inflateMultiplier;

            CalculateModifiersMultiplier(modifiers);

            _powerBottom = 0;

            _rating = GetStars(modifiers);
            _passPP = GetPassPP(_rating.Pass);
            _accSlopes = CurveUtils.GetSlopes(_accCurve);
        }

        public BeatLeaderRating GetStars(GameplayModifiers modifiers)
        {
            if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.fsAccRating, _currentMapData.modifiersRating.fsPassRating, _currentMapData.modifiersRating.fsTechRating);
            }
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Slower))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.ssAccRating, _currentMapData.modifiersRating.ssPassRating, _currentMapData.modifiersRating.ssTechRating);
            }
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.SuperFast))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.sfAccRating, _currentMapData.modifiersRating.sfPassRating, _currentMapData.modifiersRating.sfTechRating);
            }
            return new BeatLeaderRating((float)_currentMapData.accRating, (float)_currentMapData.passRating, (float)_currentMapData.techRating);
        }

        public BeatLeaderRating GetStars(List<string> modifiers)
        {
            if (modifiers.Contains("Fast"))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.fsAccRating, _currentMapData.modifiersRating.fsPassRating, _currentMapData.modifiersRating.fsTechRating);
            }
            else if (modifiers.Contains("Slower"))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.ssAccRating, _currentMapData.modifiersRating.ssPassRating, _currentMapData.modifiersRating.ssTechRating);
            }
            else if (modifiers.Contains("Super Fast"))
            {
                return new BeatLeaderRating(_currentMapData.modifiersRating.sfAccRating, _currentMapData.modifiersRating.sfPassRating, _currentMapData.modifiersRating.sfTechRating);
            }
            return new BeatLeaderRating((float)_currentMapData.accRating, (float)_currentMapData.passRating, (float)_currentMapData.techRating);
        }

        public async Task<bool> GetData(SongID songID)
        {
            try
            {
                _currentMapData = null;

                string url = "https://api.beatleader.net/map/modinterface/" + songID.id;
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(url).ConfigureAwait(false);
                using HttpContent content = res.Content;
                var str = await content.ReadAsStringAsync().ConfigureAwait(false);
                if (str != null)
                {
                    List<BLData> obj = JsonConvert.DeserializeObject<List<BLData>>(str);
                    foreach (BLData data in obj)
                    {
                        if (data.difficultyName == songID.difficulty.ToString())
                        {
                            _currentMapData = data;
                            break;
                        }
                    }
                    if (_currentMapData?.status == 3) // Ranked
                    {
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

        public double ToStars(double acc)
        {
            double passPP = 15.2f * Math.Exp(Math.Pow((float)_rating.Pass, 1 / 2.62f)) - 30f;
            if (double.IsInfinity(passPP) || double.IsNaN(passPP) || double.IsNegativeInfinity(passPP) || passPP < 0)
            {
                passPP = 0;
            }
            double accPP = Curve2(acc) * _rating.Acc * 34f;
            double techPP = Math.Exp((float)(1.9 * acc)) * 1.08f * _rating.Tech;
            double pp = 650f * Math.Pow((float)(passPP + accPP + techPP), 1.3f) / Math.Pow(650f, 1.3f);
            return pp / 52;
        }

        public double Curve2(double acc)
        {
            int i = 0;
            for (; i < baseCurve.Count; i++)
            {
                if (baseCurve[i].x <= acc)
                {
                    break;
                }
            }

            if (i == 0)
            {
                i = 1;
            }

            double middle_dis = (acc - baseCurve[i - 1].x) / (baseCurve[i].x - baseCurve[i - 1].x);
            return (float)(baseCurve[i - 1].y + middle_dis * (baseCurve[i].y - baseCurve[i - 1].y));
        }

        public List<(double x, double y)> baseCurve = new List<(double x, double y)>()
        {
                (1.0, 7.424),
                (0.999, 6.241),
                (0.9975, 5.158),
                (0.995, 4.010),
                (0.9925, 3.241),
                (0.99, 2.700),
                (0.9875, 2.303),
                (0.985, 2.007),
                (0.9825, 1.786),
                (0.98, 1.618),
                (0.9775, 1.490),
                (0.975, 1.392),
                (0.9725, 1.315),
                (0.97, 1.256),
                (0.965, 1.167),
                (0.96, 1.094),
                (0.955, 1.039),
                (0.95, 1.000),
                (0.94, 0.931),
                (0.93, 0.867),
                (0.92, 0.813),
                (0.91, 0.768),
                (0.9, 0.729),
                (0.875, 0.650),
                (0.85, 0.581),
                (0.825, 0.522),
                (0.8, 0.473),
                (0.75, 0.404),
                (0.7, 0.345),
                (0.65, 0.296),
                (0.6, 0.256),
                (0.0, 0.000)};


        public float CalculatePP(float accuracy)
        {
            float passPP = _passPP;
            float accPP = GetAccPP(_rating.Acc, accuracy);
            float techPP = GetTechPP(_rating.Tech, accuracy);
            float rawPP = Inflate(passPP + accPP + techPP) * _modifierMultiplier;
            if (float.IsInfinity(rawPP) || float.IsNaN(rawPP) || float.IsNegativeInfinity(rawPP))
            {
                rawPP = 0;
            }

            return rawPP;
        }

        private float Inflate(float pp)
        {
            if (Mathf.Approximately(_powerBottom, 0))
            {
                _powerBottom = Mathf.Pow(_inflateMultiplier, _inflateExponential);
            }

            return _inflateMultiplier * Mathf.Pow(pp, _inflateExponential) / _powerBottom;
        }

        private float GetPassPP(float passRating)
        {
            float passPP = _passMultiplier * Mathf.Exp(Mathf.Pow(passRating, _passExponential)) + _passShift;
            if (float.IsInfinity(passPP) || float.IsNaN(passPP) || float.IsNegativeInfinity(passPP) || passPP < 0)
            {
                passPP = 0;
            }

            return passPP;
        }

        private float GetAccPP(float accRating, float accuracy)
        {
            return CurveUtils.GetCurveMultiplier(_accCurve, _accSlopes, accuracy) * accRating * _accMultiplier;
        }

        private float GetTechPP(float techRating, float accuracy)
        {
            return (float) Math.Exp(_techExponentialMultiplier * accuracy) * _techMultiplier * techRating;
        }

        private void CalculateModifiersMultiplier(GameplayModifiers modifiers)
        {
            _modifierMultiplier = 1;

            if (modifiers.disappearingArrows)
            {
                _modifierMultiplier += _currentMapData.modifierValues.da;
            }
            if (modifiers.ghostNotes)
            {
                _modifierMultiplier += _currentMapData.modifierValues.gn;
            }
            if (modifiers.noArrows)
            {
                _modifierMultiplier += _currentMapData.modifierValues.na;
            }
            if (modifiers.noBombs)
            {
                _modifierMultiplier += _currentMapData.modifierValues.nb;
            }
            if (modifiers.enabledObstacleType.Equals(GameplayModifiers.EnabledObstacleType.NoObstacles))
            {
                _modifierMultiplier += _currentMapData.modifierValues.no;
            }
            if (modifiers.proMode)
            {
                _modifierMultiplier += _currentMapData.modifierValues.pm;
            }
            if (modifiers.smallCubes)
            {
                _modifierMultiplier += _currentMapData.modifierValues.sc;
            }
        }

        private void CalculateModifiersMultiplier(List<string> modifiers)
        {
            _modifierMultiplier = 1;

            if (modifiers.Contains("da"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.da;
            }
            if (modifiers.Contains("gn"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.gn;
            }
            if (modifiers.Contains("na"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.na;
            }
            if (modifiers.Contains("nb"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.nb;
            }
            if (modifiers.Contains("no"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.no;
            }
            if (modifiers.Contains("pm"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.pm;
            }
            if (modifiers.Contains("sc"))
            {
                _modifierMultiplier += _currentMapData.modifierValues.sc;
            }
        }
    }
}
