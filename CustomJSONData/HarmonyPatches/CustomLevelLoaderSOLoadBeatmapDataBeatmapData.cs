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
            //Plugin.logger.Debug("In LoadBeatmapDataBeatmapData");
            string path = Path.Combine(customLevelPath, difficultyFileName);
            //Plugin.logger.Debug("Loading " + standardLevelInfoSaveData.songName + " (" + path + ")");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                CustomBeatmapSaveData bsd = CustomBeatmapSaveData.DeserializeFromJSONString(json);
                // NOTE: logic depends on the above call always returning non-null when the vanilla version would!
                if (bsd == null)
                {
                    //Plugin.logger.Debug("CustomBeatmapSaveData was null; falling back to BeatmapDataLoader.GetBeatmapDataFromJson");
                    __result = BeatmapDataLoader.GetBeatmapDataFromJson(json, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
                }
                else if (standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
                {
                    //Plugin.logger.Debug("Loaded CustomBeatmapSaveData with CustomLevelInfoSaveData");
                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, lisd.beatsPerMinute, lisd.shuffle, lisd.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree(), lisd.customData ?? Tree());
                }
                else
                {
                    //Plugin.logger.Debug("Loaded CustomBeatmapSaveData with StandardLevelInfoSaveData");
                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), Tree(), Tree());
                }
                /*
                if (__result is CustomBeatmapData beatmapData)
                {
                    foreach (var line in beatmapData.beatmapLinesData)
                    {
                        foreach (var obj in line.beatmapObjectsData)
                        {
                            if (obj is CustomObstacleData obs)
                            {
                                Plugin.logger.Debug("Custom obstacle at " + obs.time);
                                Plugin.logger.Debug("Data:");
                                foreach (var pair in (IDictionary<string, object>)obs.customData)
                                {
                                    Plugin.logger.Debug("      \"" + pair.Key + "\": " + pair.Value);
                                }
                            }
                            else if (obj is ObstacleData ob)
                            {
                                Plugin.logger.Debug("Non-custom obstacle at " + ob.time);
                            }
                            if (obj is CustomNoteData n)
                            {
                                Plugin.logger.Debug("Custom note at " + n.time);
                                Plugin.logger.Debug("Data:");
                                foreach (var pair in (IDictionary<string, object>)n.customData)
                                {
                                    Plugin.logger.Debug("      \"" + pair.Key + "\": " + pair.Value);
                                }
                            }
                            else if (obj is NoteData no)
                            {
                                Plugin.logger.Debug("Non-custom note at " + no.time);
                            }
                        }
                    }
                }
                */
            }
            return false;
        }
    }
}
