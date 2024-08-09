using Newtonsoft.Json;
using PPCounter.Utilities;
using System;
using System.IO;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    class PPData : IInitializable
    {
        public bool DataInit { get; private set; } = false;
        public bool CurveInit { get; private set; } = false;
        [Inject] private PPDownloader _ppDownloader;

        private Leaderboards _curves = new Leaderboards();
        public Leaderboards Curves
        {
            get { return _curves; }
        }

        private static readonly string CURVE_FILE_NAME = Path.Combine(Environment.CurrentDirectory, "UserData", "PPCounter", "curves.json");

        public void Initialize()
        {
            LoadCurveFile();
            _ppDownloader.OnCurvesDownloaded += OnCurvesDownloaded;
            _ppDownloader.StartDownloadingCurves();
        }

        public void OnCurvesDownloaded(Leaderboards curves)
        {
            lock (_curves)
            {
                _curves = curves;
                CurveInit = true;
                WriteCurveFile();
            }
        }

        private void LoadCurveFile()
        {
            if (File.Exists(CURVE_FILE_NAME))
            {
                try
                {
                    lock (_curves)
                    {
                        if (!CurveInit)
                        {
                            var jsonString = File.ReadAllText(CURVE_FILE_NAME);
                            _curves = JsonConvert.DeserializeObject<Leaderboards>(jsonString);
                            CurveInit = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.log.Error($"Failed to deserialize JSON: {ex.Message}");
                }
            }
        }

        private void WriteCurveFile()
        {
            OSUtils.WriteFile(_curves, CURVE_FILE_NAME);
        }
    }
}
