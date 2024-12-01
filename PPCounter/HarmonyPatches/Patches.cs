using HarmonyLib;
using PPCounter.UI;
using PPCounter.Utilities;

namespace PPCounter.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
    internal class StandardLevelDetailViewPatch
    {
        static void Postfix(StandardLevelDetailView __instance)
        {
            PPView.hash = BeatmapsUtil.GetHashOfLevel(__instance._beatmapLevel);
            PPView.difficulty = __instance.beatmapKey.difficulty;
        }
    }
}
