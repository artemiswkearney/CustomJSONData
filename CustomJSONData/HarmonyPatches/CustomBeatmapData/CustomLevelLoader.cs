namespace CustomJSONData.HarmonyPatches
{
    using CustomJSONData.CustomBeatmap;
    using CustomJSONData.CustomLevelInfo;
    using HarmonyLib;
    using static CustomJSONData.Trees;

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadBeatmapDataBeatmapData
    {
        private static void Postfix(ref BeatmapData __result, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            if (__result != null && __result is CustomBeatmapData customBeatmapData && standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
            {
                customBeatmapData.SetBeatmapCustomData(at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree());
                customBeatmapData.SetLevelCustomData(lisd.customData ?? Tree());
            }
        }
    }
}
