using HarmonyLib;
using Newtonsoft.Json;
using CustomJSONData.CustomBeatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CustomJSONData.CustomLevelInfo;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyExtensions
{
    [HarmonyPatch(typeof(BeatmapDataLoader),
        "GetBeatmapDataFromJson",
        new Type[] {
            typeof(string),
            typeof(float),
            typeof(float),
            typeof(float),
        })]
    class BeatmapDataLoaderGetBeatmapDataFromJson
    {
        public static bool Prefix(BeatmapDataLoader __instance, string json, float startBPM, float shuffle, float shufflePeriod, ref BeatmapData __result)
        {
            //Plugin.logger.Debug("In GetBeatmapDataFromJson");

            CustomBeatmapSaveData saveData = CustomBeatmapSaveData.DeserializeFromJSONString(json);
            if (saveData == null) return true;

            __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(saveData.notes, saveData.obstacles, saveData.events, startBPM, shuffle, shufflePeriod, saveData.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), Tree(), Tree(), __instance);
            if (!(__result is CustomBeatmapData)) return true;

            CustomBeatmapData beatmapData = __result as CustomBeatmapData;
            return false;
        }
    }
}
