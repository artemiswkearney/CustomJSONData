namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    ////using System.Diagnostics;
    using System.IO;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;
    using static BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData;

    /*[HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadingStopwatch
    {
        private static void Prefix(out Stopwatch __state)
        {
            __state = Stopwatch.StartNew();
        }

        private static void Postfix(Stopwatch __state)
        {
            __state.Stop();
            Logger.Log($"Loading took: {__state.ElapsedMilliseconds} ms");
        }
    }*/

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadCustomLevelInfoSaveData")]
    internal class CustomLevelLoaderLoadCustomLevelInfoSaveData
    {
        private static bool Prefix(ref StandardLevelInfoSaveData __result, string customLevelPath)
        {
            string path = Path.Combine(customLevelPath, "Info.dat");
            if (File.Exists(path))
            {
                __result = CustomLevelInfoSaveData.Deserialize(path);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadBeatmapDataBeatmapData
    {
        private static bool Prefix(ref BeatmapData __result, string customLevelPath, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData, BeatmapDataLoader ____beatmapDataLoader)
        {
            string path = Path.Combine(customLevelPath, difficultyFileName);
            if (File.Exists(path))
            {
                CustomBeatmapSaveData saveData = CustomBeatmapSaveData.Deserialize(path);

                List<BeatmapSaveData.NoteData> notes = saveData.notes;
                List<BeatmapSaveData.ObstacleData> obstacles = saveData.obstacles;
                List<BeatmapSaveData.EventData> events = saveData.events;
                List<BeatmapSaveData.WaypointData> waypoints = saveData.waypoints;
                BeatmapSaveData.SpecialEventKeywordFiltersData specialEventsKeywordFilters = saveData.specialEventsKeywordFilters;

                BeatmapSaveData = saveData;
                if (standardLevelInfoSaveData is CustomLevelInfoSaveData customLevelInfoSaveData)
                {
                    BeatmapCustomData = customLevelInfoSaveData.beatmapCustomDatasByFilename[difficultyFileName];
                    LevelCustomData = customLevelInfoSaveData.customData;
                }

                __result = ____beatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(notes, waypoints, obstacles, events, specialEventsKeywordFilters, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
            }

            return false;
        }

        private static void Postfix(BeatmapData __result, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            BeatmapSaveData = null;
            BeatmapCustomData = null;
            LevelCustomData = null;
        }
    }
}
