using CustomJSONData.CustomBeatmap;
using CustomJSONData.CustomLevelInfo;
using HarmonyLib;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadBeatmapDataBeatmapData
    {
        private static void Postfix(ref BeatmapData __result, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            if (__result != null && __result is CustomBeatmapData customBeatmapData && standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
            {
                customBeatmapData.customEventData = new CustomEventData[0];
                customBeatmapData.beatmapCustomData = at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree();
                customBeatmapData.levelCustomData = lisd.customData ?? Tree();
            }
        }
    }
}