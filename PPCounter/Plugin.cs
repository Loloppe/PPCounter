using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using PPCounter.Settings;
using PPCounter.Utilities;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace PPCounter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }

        internal static bool BeatLeaderInstalled = false;
        internal static IPALogger log;
        internal static Harmony harmony;

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenject)
        {
            instance = this;
            log = logger;
            zenject.Install<Installers.DataInstaller>(Location.App);
            zenject.Install<Installers.CalculatorsInstaller>(Location.App);
            zenject.Install<Installers.PPCounterInstaller>(Location.StandardPlayer, Location.MultiPlayer);
            zenject.Install<Installers.MenuInstaller>(Location.Menu);
            PluginSettings.Instance = config.Generated<PluginSettings>();
            harmony = new Harmony("Loloppe.BeatSaber.PPCounter");
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            RenewSettings();
        }

        private void RenewSettings()
        {
            var enumCount = Enum.GetValues(typeof(PPCounters)).Length;
            if (PluginSettings.Instance.numCounters < enumCount)
            {
                List<PPCounters> order = SettingsUtils.GetCounterOrder(PluginSettings.Instance.preferredOrder, PluginSettings.Instance.numCounters);
                foreach (PPCounters counter in Enum.GetValues(typeof(PPCounters)))
                {
                    if (!order.Contains(counter))
                    {
                        order.Add(counter);
                    }
                }

                PluginSettings.Instance.preferredOrder = SettingsUtils.GetPreferredOrderNumber(order);
                PluginSettings.Instance.numCounters = enumCount;
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchSelf();
        }
    }
}
