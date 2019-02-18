using Harmony;
using Newtonsoft.Json;
using CustomJSONData.CustomBeatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

            __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(saveData.notes, saveData.obstacles, saveData.events, saveData.beatsPerMinute, shuffle, shufflePeriod, saveData.customData, saveData._warnings, saveData._suggestions, saveData._requirements, saveData._colorLeft, saveData._colorRight, saveData._noteJumpStartBeatOffset);
            if (!(__result is CustomBeatmapData)) return true;
            CustomBeatmapData beatmapData = __result as CustomBeatmapData;
            if (beatmapData.customData == null)
            {
                Console.WriteLine("[CustomJSONData] Loaded beatmap with no custom data.");
            }
            else
            {
                Console.WriteLine("[CustomJSONData] Loaded beatmap with custom data!");
                Console.WriteLine("[CustomJSONData] Custom data type: " + ((CustomBeatmapData)__result).customData);
                foreach(var pair in (IDictionary<String, Object>)((CustomBeatmapData)__result).customData)
                {
                    Console.WriteLine("\"" + pair.Key + "\": " + pair.Value);
                }
                Console.WriteLine("Other CustomData locations ignored.");
            }
            if (beatmapData.warnings != null)
            {
                Console.WriteLine("[CustomJSONData] Warnings:");
                beatmapData.warnings.Do(Console.WriteLine);
            }
            if (beatmapData.suggestions != null)
            {
                Console.WriteLine("[CustomJSONData] Suggestions:");
                beatmapData.suggestions.Do(Console.WriteLine);
            }
            if (beatmapData.requirements != null)
            {
                Console.WriteLine("[CustomJSONData] Requirements:");
                beatmapData.requirements.Do(Console.WriteLine);
            }
            if (beatmapData.leftColor != null && beatmapData.rightColor != null)
            {
                Console.WriteLine("[CustomJSONData] Colors:");
                Console.WriteLine("Left: " + beatmapData.leftColor.r + ", " + beatmapData.leftColor.g + ", " + beatmapData.leftColor.b);
                Console.WriteLine("Right: " + beatmapData.rightColor.r + ", " + beatmapData.rightColor.g + ", " + beatmapData.rightColor.b);
            }
            if (beatmapData.noteJumpStartBeatOffset != null)
            {
                Console.WriteLine("[CustomJSONData] noteJumpStartBeatOffset: " + beatmapData.noteJumpStartBeatOffset);
            }
            return false;
        }
    }
}
