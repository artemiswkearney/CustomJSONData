using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomJSONData.CustomLevelInfo;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelInfoSaveData),
        "DeserializeFromJSONString")]
    class StandardLevelInfoSaveDataDeserializeFromJSONString
    {
        public static void Postfix(string stringData, ref StandardLevelInfoSaveData __result)
        {
            //Plugin.logger.Debug("In DeserializeFromJSONString");
            if (__result == null)
            {
                //Plugin.logger.Info("Result null with data:\n" + stringData);
                return;
            }
            __result = CustomLevelInfoSaveData.DeserializeFromJSONString(stringData, __result);
            /*
            if (__result is CustomLevelInfoSaveData info)
            {
                Plugin.logger.Info(info.songName + ":");
                foreach (var pair in (IDictionary<string, object>)info.customData)
                {
                    Plugin.logger.Info("  \"" + pair.Key + "\": " + pair.Value);
                }
                foreach (var set in info.difficultyBeatmapSets)
                {
                    foreach (var beatmap in set.difficultyBeatmaps)
                    {
                        if (beatmap is CustomLevelInfoSaveData.DifficultyBeatmap b)
                        {
                            Plugin.logger.Info("    " + set.beatmapCharacteristicName + " -> " + b.difficulty + ":");
                            foreach (var pair in (IDictionary<string, object>)b.customData)
                            {
                                Plugin.logger.Info("      \"" + pair.Key + "\": " + pair.Value);
                            }
                        }
                    }
                }
            }
            */
        }
    }
}
