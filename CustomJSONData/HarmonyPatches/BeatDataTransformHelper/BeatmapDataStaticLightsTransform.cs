using HarmonyLib;
using static CustomJSONData.HarmonyPatches.BeatDataTransformHelperHelper;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataStaticLightsTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal class BeatmapDataStaticLightsTransformCreateTransformedData
    {
        private static void Postfix(ref BeatmapData __result, BeatmapData beatmapData)
        {
            PostfixHelper(ref __result, beatmapData);
        }
    }
}