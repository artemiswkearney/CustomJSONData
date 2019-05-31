using Harmony;
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
        public static bool Prefix(string json, float beatsPerMinute, float shuffle, float shufflePeriod, ref BeatmapData __result)
        {

            CustomBeatmapSaveData saveData = CustomBeatmapSaveData.DeserializeFromJSONString(json);
            if (saveData == null) return true;

            __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(saveData.notes, saveData.obstacles, saveData.events, beatsPerMinute, shuffle, shufflePeriod, saveData.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), Tree(), Tree());
            if (!(__result is CustomBeatmapData)) return true;

            CustomBeatmapData beatmapData = __result as CustomBeatmapData;
            return false;
        }
    }
}
