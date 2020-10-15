namespace CustomJSONData.HarmonyPatches
{
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapData))]
    [HarmonyPatch("GetCopy")]
    internal class BeatmapDataGetCopy
    {
        private static void Postfix(BeatmapData __instance, ref BeatmapData __result)
        {
            if (__instance is CustomBeatmapData customBeatmapData)
            {
                __result = customBeatmapData.GetCopy();
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapData))]
    [HarmonyPatch("GetCopyWithoutEvents")]
    internal class BeatmapDataGetCopyWithoutEvents
    {
        private static void Postfix(BeatmapData __instance, ref BeatmapData __result)
        {
            if (__instance is CustomBeatmapData customBeatmapData)
            {
                __result = customBeatmapData.GetCopyWithoutEvents();
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapData))]
    [HarmonyPatch("GetCopyWithoutBeatmapObjects")]
    internal class BeatmapDataGetCopyWithoutBeatmapObjects
    {
        private static void Postfix(BeatmapData __instance, ref BeatmapData __result)
        {
            if (__instance is CustomBeatmapData customBeatmapData)
            {
                __result = customBeatmapData.GetCopyWithoutBeatmapObjects();
            }
        }
    }
}
