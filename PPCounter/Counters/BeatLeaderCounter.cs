﻿using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPCounter.Calculators;
using PPCounter.Settings;
using PPCounter.Utilities;
using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Counters
{
    internal class BeatLeaderCounter : IPPCounter
    {
        [Inject] private BeatLeaderCalculator beatLeaderUtils;

        private SongID _songID;

        private ImageView _image;
        private TMP_Text _text;

        public Sprite GetIcon()
        {
            return BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("PPCounter.Images.BeatLeader.png");
        }

        public bool IsActive(Structs.SongID songID)
        {
            return beatLeaderUtils.GetData(songID).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void InitData(Structs.SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, Structs.Leaderboards leaderboards)
        {
            beatLeaderUtils.SetCurve(leaderboards.BeatLeader, gameplayModifiers);
        }

        public void InitCounter(CanvasUtility canvasUtility, CustomConfigModel settings, int counterIndex)
        {
            float iconOffset = 0;

            if (PluginSettings.Instance.showIcons)
            {
                _image = CounterUtils.CreateIcon(canvasUtility, settings, GetIcon(), counterIndex);
                iconOffset = CounterUtils.ICON_OFFSET;
            }

            _text = canvasUtility.CreateTextFromSettings(settings, CounterUtils.GetTextOffset(counterIndex));
            _text.fontSize = 3;
        }

        public void UpdateCounter(float acc, bool failed = false)
        {
            var pp = 0f;
            if (!failed) pp = beatLeaderUtils.CalculatePP(acc);

            var ppString = pp.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
            _text.text = $"{ppString}pp";
        }
    }
}
