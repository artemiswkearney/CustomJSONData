using HarmonyLib;
using System.Collections.Generic;
using static CustomJSONData.HarmonyPatches.BeatDataTransformHelperHelper;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataObstaclesAndBombsTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal class BeatmapDataObstaclesAndBombsTransformCreateTransformedData
    {
        private static void Postfix(ref BeatmapData __result, BeatmapData beatmapData)
        {
            PostfixHelper(ref __result, beatmapData);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return TranspilerHelper(instructions);
        }
    }
}
