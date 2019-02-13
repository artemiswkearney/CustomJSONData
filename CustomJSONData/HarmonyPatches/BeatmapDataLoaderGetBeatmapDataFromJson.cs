using Harmony;
using Newtonsoft.Json;
using CustomJSONData.CustomBeatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static bool Prefix(string json, float shuffle, float shufflePeriod, ref BeatmapData __result)
        {
            CustomBeatmapSaveData saveData = CustomBeatmapSaveData.DeserializeFromJSONString(json);
            if (saveData == null) return true;

            __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(saveData.notes, saveData.obstacles, saveData.events, saveData.beatsPerMinute, shuffle, shufflePeriod, saveData.customData);
            Console.WriteLine("[CustomJSONData] Loaded beatmap with custom data!");
            foreach(var pair in (IDictionary<String, Object>)((CustomBeatmapData)__result).customData)
            {
                Console.WriteLine("\"" + pair.Key + "\": " + pair.Value);
            }
            Console.WriteLine("Other CustomData locations ignored.");
            return false;
        }
    }
}
