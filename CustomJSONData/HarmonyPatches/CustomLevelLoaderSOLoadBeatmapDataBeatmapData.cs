using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomJSONData.CustomBeatmap;
using CustomJSONData.CustomLevelInfo;
using Harmony;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLevelLoaderSO), "LoadBeatmapDataBeatmapData")]
    class CustomLevelLoaderSOLoadBeatmapDataBeatmapData // sic
    {
        static bool Prefix(string customLevelPath, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData, ref BeatmapData __result)
        {
            string path = Path.Combine(customLevelPath, difficultyFileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                CustomBeatmapSaveData bsd = CustomBeatmapSaveData.DeserializeFromJSONString(json);
                // NOTE: logic depends on the above call always returning non-null when the vanilla version would!
                if (bsd == null)
                {
                    __result = BeatmapDataLoader.GetBeatmapDataFromJson(json, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
                }
                else if (standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
                {

                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, lisd.beatsPerMinute, lisd.shuffle, lisd.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), lisd.beatmapCustomDatasByFilename.at(difficultyFileName) ?? Tree(), lisd.customData ?? Tree());
                }
                else
                {
                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), Tree(), Tree());
                }
            }
            __result = null;
            return false;
        }
    }
}
