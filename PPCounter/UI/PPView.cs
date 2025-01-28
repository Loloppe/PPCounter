using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPA.Config.Stores;
using PPCounter.Calculators;
using PPCounter.Data;
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

        public List<object> SpeedModifiers = new object[] { "Slower", "Normal", "Fast", "Super Fast" }.ToList();

        [UIValue("sfs")]
        public string SFS { get; set; } = "SFS";

        [UIValue("fs")]
        public string FS { get; set; } = "FS";

        [UIValue("normal")]
        public string Normal { get; set; } = "Normal";

        [UIValue("ss")]
        public string SS { get; set; } = "SS";

        [UIValue("sfsstar")]
        public string SFSStar { get; set; } = "⭐";

        [UIValue("fsstar")]
        public string FSStar { get; set; } = "⭐";

        [UIValue("normalstar")]
        public string NormalStar { get; set; } = "⭐";

        [UIValue("ssstar")]
        public string SSStar { get; set; } = "⭐";

        [UIValue("pp")]
        public string PP { get; set; } = "pp";

        [UIAction("pp_clicked")]
        public void PP_Clicked()
        {
            if (hash == null)
            {
                SFS = "SFS:";
                SFSStar = "⭐";
                FS = "FS:";
                FSStar = "⭐";
                Normal = "Normal:";
                NormalStar = "⭐";
                SS = "SS:";
                SSStar = "⭐";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SFS)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FS)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Normal)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SS)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SFSStar)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FSStar)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormalStar)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SSStar)));
                return;
            }
            SongID songID = new SongID(hash, difficulty);

            BeatLeaderCalculator beatLeaderUtils = new BeatLeaderCalculator();
            if (beatLeaderUtils.GetData(songID).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                try
                {
                    foreach(string modifier in SpeedModifiers)
                    {
                        beatLeaderUtils.SetCurve(ppData.Curves.BeatLeader, new List<string>() { modifier });
                        var star = Math.Round(beatLeaderUtils.ToStars(0.96),2);
                        var text = beatLeaderUtils.CalculatePP(Accuracy / 100);
                        var ppString = text.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
                        switch (modifier)
                        {
                            case "Super Fast":
                                SFS = $"SFS: {ppString}pp";
                                SFSStar = $"{star}⭐";
                                break;
                            case "Fast":
                                FS = $"FS: {ppString}pp";
                                FSStar = $"{star}⭐";
                                break;
                            case "Normal":
                                Normal = $"Normal: {ppString}pp";
                                NormalStar = $"{star}⭐";
                                break;
                            case "Slower":
                                SS = $"SS: {ppString}pp";
                                SSStar = $"{star}⭐";
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.log.Error(e);
                }
            }
            else
            {
                SFS = "SFS:";
                SFSStar = "⭐";
                FS = "FS:";
                FSStar = "⭐";
                Normal = "Normal:";
                NormalStar = "⭐";
                SS = "SS:";
                SSStar = "⭐";
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SFS)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FS)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Normal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SS)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SFSStar)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FSStar)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormalStar)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SSStar)));
        }
    }
}