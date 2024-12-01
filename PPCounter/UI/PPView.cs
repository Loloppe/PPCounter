using BeatmapSaveDataVersion4;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPA.Config.Stores;
using PPCounter.Calculators;
using PPCounter.Data;
using PPCounter.HarmonyPatches;
using PPCounter.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Zenject;
using static PPCounter.Utilities.Structs;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PPCounter.UI
{
    internal class PPView : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [Inject] private PPData ppData;
        public event PropertyChangedEventHandler PropertyChanged;
        public static string hash = null;
        public static BeatmapDifficulty difficulty;

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab("PPCounter", "PPCounter.UI.ppview.bsml", this, MenuType.Solo);
        }

        public void Dispose()
        {
            if (GameplaySetup.Instance != null)
            {
                GameplaySetup.Instance.RemoveTab("PPCounter");
            }
        }

        [UIValue("accuracy")]
        public float Accuracy { get; set; } = 96f;

        [UIValue("speedmodifier")]
        public string SpeedModifier { get; set; } = "Normal";

        [UIValue("speedmodifiers")]
        public List<object> SpeedModifiers = new object[] { "Slower", "Normal", "Fast", "Super Fast" }.ToList();

        [UIValue("pp")]
        public string PP { get; set; } = "pp";

        [UIAction("pp_clicked")]
        public void PP_Clicked()
        {
            if (hash == null)
            {
                PP = "X";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PP)));
                return;
            }
            SongID songID = new SongID(hash, difficulty);
            List<string> modifiers = new List<string>
            {
                SpeedModifier
            };

            BeatLeaderCalculator beatLeaderUtils = new BeatLeaderCalculator();
            if (beatLeaderUtils.GetData(songID).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                try
                {
                    beatLeaderUtils.SetCurve(ppData.Curves.BeatLeader, modifiers);
                    var text = beatLeaderUtils.CalculatePP(Accuracy / 100);
                    var ppString = text.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
                    PP = $"{ppString}pp";
                }
                catch (Exception e)
                {
                    Plugin.log.Error(e);
                }
            }
            else PP = "X";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PP)));
        }
    }
}