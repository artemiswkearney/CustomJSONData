using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelDetailView), "SetContent")]
    class StandardLevelDetailViewSetContent
    {
        public static bool Prefix(StandardLevelDetailView __instance, IBeatmapLevel level, BeatmapDifficulty defaultDifficulty, IPlayer player, bool showPlayerStats)
        {
            return true;
        }
    }
}
