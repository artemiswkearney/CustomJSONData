﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomJSONData.CustomBeatmap;
using CustomJSONData.CustomLevelInfo;
using HarmonyLib;
using static CustomJSONData.Trees;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLevelLoader), "LoadBeatmapDataBeatmapData")]
    class CustomLevelLoaderLoadBeatmapDataBeatmapData // sic
    {
        static bool Prefix(BeatmapDataLoader ____beatmapDataLoader, string customLevelPath, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData, ref BeatmapData __result)
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
                    __result = ____beatmapDataLoader.GetBeatmapDataFromJson(json, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
                }
                else if (standardLevelInfoSaveData is CustomLevelInfoSaveData lisd)
                {
                    //Plugin.logger.Debug("Loaded CustomBeatmapSaveData with CustomLevelInfoSaveData");
                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, lisd.beatsPerMinute, lisd.shuffle, lisd.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), at(lisd.beatmapCustomDatasByFilename, difficultyFileName) ?? Tree(), lisd.customData ?? Tree(), ____beatmapDataLoader);
                }
                else
                {
                    //Plugin.logger.Debug("Loaded CustomBeatmapSaveData with StandardLevelInfoSaveData");
                    __result = CustomBeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(bsd.notes, bsd.obstacles, bsd.events, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod, bsd.customEvents ?? new List<CustomBeatmapSaveData.CustomEventData>(), Tree(), Tree(), ____beatmapDataLoader);
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
                    foreach (var pair in beatmapData.customEventData)
                    {
                        Plugin.logger.Debug("Custom event \"" + pair.Key + "\":");
                        foreach (var e in pair.Value)
                        {
                            Plugin.logger.Debug("    " + e.time + ":");
                            foreach (var innerPair in (IDictionary<string, object>)e.data)
                            {
                                Plugin.logger.Debug("        \"" + innerPair.Key + "\": " + innerPair.Value);
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
