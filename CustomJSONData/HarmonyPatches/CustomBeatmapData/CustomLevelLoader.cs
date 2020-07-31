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
                __result = new CustomBeatmapData(
                    __result.beatmapLinesData, 
                    __result.beatmapEventData, 
                    customBeatmapData.customEventData,
                    customBeatmapData.customData,
                    at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree(),
                    lisd.customData ?? Tree());
            }
        }
    }
}
